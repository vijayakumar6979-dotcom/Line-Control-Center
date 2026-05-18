using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Settings;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns daily DPM aggregates for the past <paramref name="Days"/> days.</summary>
public sealed record GetFniDpmHistoryQuery(
    string  Customer,
    string? Family,
    int     Days = 21) : IRequest<Result<IReadOnlyList<FniDpmDailyDto>>>;

public sealed class GetFniDpmHistoryQueryHandler
    : IRequestHandler<GetFniDpmHistoryQuery, Result<IReadOnlyList<FniDpmDailyDto>>>
{
    private readonly IPostgresqlDbContext          _db;
    private readonly IOptionsMonitor<DpmSettings>  _dpmOptions;

    public GetFniDpmHistoryQueryHandler(
        IPostgresqlDbContext         db,
        IOptionsMonitor<DpmSettings> dpmOptions)
    {
        _db         = db;
        _dpmOptions = dpmOptions;
    }

    public async Task<Result<IReadOnlyList<FniDpmDailyDto>>> Handle(
        GetFniDpmHistoryQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            return Result.Failure<IReadOnlyList<FniDpmDailyDto>>(
                new Error("FNI.CustomerRequired", "Customer is required."));

        var settings     = _dpmOptions.CurrentValue;
        var fniSteps     = settings.FniStepNames;
        var filterProc   = settings.FilterProcessLoop;
        var filterTest   = settings.FilterTestLoop;
        var allFamilies  = string.IsNullOrWhiteSpace(request.Family);
        // end_time is "timestamp WITHOUT time zone" — use Unspecified Kind so
        // Npgsql sends wall-clock values as-is instead of converting Local→UTC.
        var since        = DateTime.SpecifyKind(DateTime.Today.AddDays(-request.Days), DateTimeKind.Unspecified);
        var now          = DateTime.SpecifyKind(DateTime.Now,                          DateTimeKind.Unspecified);

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
                return Result.Success<IReadOnlyList<FniDpmDailyDto>>(Array.Empty<FniDpmDailyDto>());
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
               && m.EndTime.Value    >= since
               && m.EndTime.Value    <= now
            select new { m.EndTime, m.Status }
        ).ToListAsync(ct);

        var result = rows
            .GroupBy(r => DateOnly.FromDateTime(r.EndTime!.Value))
            .Select(g =>
            {
                var total = g.Count();
                var fail  = g.Count(r => !IsPass(r.Status));
                return new FniDpmDailyDto(
                    g.Key,
                    total,
                    fail,
                    total > 0 ? Math.Round((double)fail / total * 1_000_000, 1) : 0);
            })
            .OrderBy(d => d.Day)
            .ToList();

        return Result.Success<IReadOnlyList<FniDpmDailyDto>>(result);
    }

    private static bool IsPass(string? s) =>
        string.Equals((s ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase);
}
