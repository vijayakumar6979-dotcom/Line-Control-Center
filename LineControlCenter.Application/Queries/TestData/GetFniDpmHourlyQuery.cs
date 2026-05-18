using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Settings;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns hourly DPM aggregates for a specific date (defaults to today).</summary>
public sealed record GetFniDpmHourlyQuery(
    string    Customer,
    string?   Family,
    DateOnly? Date = null) : IRequest<Result<IReadOnlyList<FniDpmHourlyDto>>>;

public sealed class GetFniDpmHourlyQueryHandler
    : IRequestHandler<GetFniDpmHourlyQuery, Result<IReadOnlyList<FniDpmHourlyDto>>>
{
    private readonly IPostgresqlDbContext          _db;
    private readonly IOptionsMonitor<DpmSettings>  _dpmOptions;

    public GetFniDpmHourlyQueryHandler(
        IPostgresqlDbContext         db,
        IOptionsMonitor<DpmSettings> dpmOptions)
    {
        _db         = db;
        _dpmOptions = dpmOptions;
    }

    public async Task<Result<IReadOnlyList<FniDpmHourlyDto>>> Handle(
        GetFniDpmHourlyQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            return Result.Failure<IReadOnlyList<FniDpmHourlyDto>>(
                new Error("FNI.CustomerRequired", "Customer is required."));

        var settings     = _dpmOptions.CurrentValue;
        var fniSteps     = settings.FniStepNames;
        var filterProc   = settings.FilterProcessLoop;
        var filterTest   = settings.FilterTestLoop;
        var allFamilies  = string.IsNullOrWhiteSpace(request.Family);
        // end_time is "timestamp WITHOUT time zone" — use Unspecified Kind so
        // Npgsql sends wall-clock values as-is instead of converting Local→UTC.
        var date         = request.Date ?? DateOnly.FromDateTime(DateTime.Today);
        var dayStart     = DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Unspecified);
        var dayEnd       = date == DateOnly.FromDateTime(DateTime.Today)
                            ? DateTime.SpecifyKind(DateTime.Now,                         DateTimeKind.Unspecified)
                            : DateTime.SpecifyKind(date.ToDateTime(TimeOnly.MaxValue),   DateTimeKind.Unspecified);

        List<long> familyIds = [];
        if (!allFamilies)
        {
            familyIds = await _db.MesFamilies
                .AsNoTracking()
                .Where(f => f.Family == request.Family)
                .Select(f => (long)f.FamilyId)
                .Distinct()
                .ToListAsync(ct);

            if (familyIds.Count == 0)
                return Result.Success<IReadOnlyList<FniDpmHourlyDto>>(Array.Empty<FniDpmHourlyDto>());
        }

        var rows = await (
            from m  in _db.MesMains.AsNoTracking()
            join rs in _db.MesRouteSteps.AsNoTracking() on m.RouteStepId equals rs.RouteStepId
            join c  in _db.MesCustomers.AsNoTracking()  on m.CustId       equals (int?)c.CustomerId
            where c.CustomerName     == request.Customer
               && fniSteps.Contains(rs.StepInstance ?? "")
               && (!filterTest || m.TestLoop == 1)
               && (!filterProc || m.ProcessLoop == 1)
               && (allFamilies || familyIds.Contains(m.FamilyId!.Value))
               && m.EndTime.HasValue
               && m.EndTime.Value    >= dayStart
               && m.EndTime.Value    <= dayEnd
            select new { m.EndTime, m.Status }
        ).ToListAsync(ct);

        // Build all 24 hour slots (only up to current hour for today)
        var maxHour = date == DateOnly.FromDateTime(DateTime.Today) ? DateTime.Now.Hour : 23;

        var result = Enumerable.Range(0, maxHour + 1)
            .Select(h =>
            {
                var bucket = rows.Where(r => r.EndTime!.Value.Hour == h).ToList();
                var total  = bucket.Count;
                var fail   = bucket.Count(r => !IsPass(r.Status));
                return new FniDpmHourlyDto(
                    h, total, fail,
                    total > 0 ? Math.Round((double)fail / total * 1_000_000, 1) : 0);
            })
            .ToList();

        return Result.Success<IReadOnlyList<FniDpmHourlyDto>>(result);
    }

    private static bool IsPass(string? s) =>
        string.Equals((s ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase);
}
