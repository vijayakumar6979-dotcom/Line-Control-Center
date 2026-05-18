using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Settings;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns DPM ranked per family for the given date range.</summary>
public sealed record GetFniDpmFamilyRankingQuery(
    string   Customer,
    DateTime From,
    DateTime To) : IRequest<Result<IReadOnlyList<FniDpmFamilyRankDto>>>;

public sealed class GetFniDpmFamilyRankingQueryHandler
    : IRequestHandler<GetFniDpmFamilyRankingQuery, Result<IReadOnlyList<FniDpmFamilyRankDto>>>
{
    private readonly IPostgresqlDbContext          _db;
    private readonly IOptionsMonitor<DpmSettings>  _dpmOptions;

    public GetFniDpmFamilyRankingQueryHandler(
        IPostgresqlDbContext         db,
        IOptionsMonitor<DpmSettings> dpmOptions)
    {
        _db         = db;
        _dpmOptions = dpmOptions;
    }

    public async Task<Result<IReadOnlyList<FniDpmFamilyRankDto>>> Handle(
        GetFniDpmFamilyRankingQuery request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            return Result.Failure<IReadOnlyList<FniDpmFamilyRankDto>>(
                new Error("FNI.CustomerRequired", "Customer is required."));

        var settings   = _dpmOptions.CurrentValue;
        var fniSteps   = settings.FniStepNames;
        var filterProc = settings.FilterProcessLoop;
        var filterTest = settings.FilterTestLoop;

        var rows = await (
            from m  in _db.MesMains.AsNoTracking()
            join rs in _db.MesRouteSteps.AsNoTracking() on m.RouteStepId equals rs.RouteStepId
            join c  in _db.MesCustomers.AsNoTracking()  on m.CustId       equals (int?)c.CustomerId
            join f  in _db.MesFamilies.AsNoTracking()   on m.FamilyId     equals (long?)f.FamilyId
            where c.CustomerName     == request.Customer
               && fniSteps.Contains(rs.StepInstance ?? "")
               && (!filterTest || m.TestLoop == 1)
               && (!filterProc || m.ProcessLoop == 1)
               && m.EndTime.HasValue
               && m.EndTime.Value    >= request.From
               && m.EndTime.Value    <= request.To
            select new { FamilyName = f.Family ?? "UNKNOWN", m.Status }
        ).ToListAsync(ct);

        var result = rows
            .GroupBy(r => r.FamilyName)
            .Select(g =>
            {
                var total = g.Count();
                var fail  = g.Count(r => !IsPass(r.Status));
                return new FniDpmFamilyRankDto(
                    g.Key, total, fail,
                    total > 0 ? Math.Round((double)fail / total * 1_000_000, 1) : 0);
            })
            .OrderByDescending(r => r.DpmRate)
            .ToList();

        return Result.Success<IReadOnlyList<FniDpmFamilyRankDto>>(result);
    }

    private static bool IsPass(string? s) =>
        string.Equals((s ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase);
}
