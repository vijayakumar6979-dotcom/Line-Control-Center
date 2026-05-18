using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Settings;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LineControlCenter.Application.Queries.TestData;

public sealed record GetFniDpmTodayQuery(
    string Customer,
    string? Family) : IRequest<Result<FniYieldSummaryDto>>;

public sealed class GetFniDpmTodayQueryHandler
    : IRequestHandler<GetFniDpmTodayQuery, Result<FniYieldSummaryDto>>
{
    private readonly IPostgresqlDbContext _db;
    private readonly ILogger<GetFniDpmTodayQueryHandler> _logger;
    private readonly IOptionsMonitor<DpmSettings> _dpmOptions;

    public GetFniDpmTodayQueryHandler(
        IPostgresqlDbContext db,
        ILogger<GetFniDpmTodayQueryHandler> logger,
        IOptionsMonitor<DpmSettings> dpmOptions)
    {
        _db         = db;
        _logger     = logger;
        _dpmOptions = dpmOptions;
    }

    public async Task<Result<FniYieldSummaryDto>> Handle(
        GetFniDpmTodayQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            return Result.Failure<FniYieldSummaryDto>(
                new Error("FNI.CustomerRequired", "Customer is required for FNI DPM query."));

        // end_time is stored as "timestamp WITHOUT time zone" in PostgreSQL.
        // Npgsql 6+ converts DateTime(Kind=Local) to UTC before sending, which shifts
        // the window by +8 h on MYT servers and inflates today's count.
        // Specifying Unspecified tells Npgsql to send the wall-clock value as-is.
        var todayStart  = DateTime.SpecifyKind(DateTime.Today,         DateTimeKind.Unspecified);
        var now         = DateTime.SpecifyKind(DateTime.Now,           DateTimeKind.Unspecified);
        var allFamilies = string.IsNullOrWhiteSpace(request.Family);

        var settings    = _dpmOptions.CurrentValue;
        var fniSteps    = settings.FniStepNames;
        var filterProc  = settings.FilterProcessLoop;
        var filterTest  = settings.FilterTestLoop;

        _logger.LogInformation(
            "FNI DPM Today settings: Steps=[{Steps}], FilterProc={FilterProc}, FilterTest={FilterTest}, Range=[{Start} .. {End}]",
            string.Join(",", fniSteps), filterProc, filterTest, todayStart, now);

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
                return Result.Success(new FniYieldSummaryDto(0, 0, 0, 0, []));
        }

        var baseQuery =
            from m in _db.MesMains.AsNoTracking()
            join rs in _db.MesRouteSteps.AsNoTracking()
                on m.RouteStepId equals rs.RouteStepId
            join c in _db.MesCustomers.AsNoTracking()
                on m.CustId equals (int?)c.CustomerId
            join mf in _db.MesFamilies.AsNoTracking()
                on m.FamilyId equals (long?)mf.FamilyId into mfj
            from mf in mfj.DefaultIfEmpty()
            where c.CustomerName  == request.Customer
               && rs.CustId       == m.CustId
               && rs.CustId       == (int?)c.CustomerId
               && fniSteps.Contains(rs.StepInstance ?? "")
               && (!filterTest || m.TestLoop == 1)
               && (!filterProc || m.ProcessLoop == 1)
               && (allFamilies || familyIds.Contains(m.FamilyId!.Value))
               && m.EndTime.HasValue
               && m.EndTime >= todayStart
               && m.EndTime <= now
            select new
            {
                m.SerialNumber,
                m.StartTime,
                m.EndTime,
                m.Status,
                CustomerName  = c.CustomerName,
                Family        = mf.Family ?? "UNKNOWN",
                StepInstance  = rs.StepInstance
            };

        var rows = await baseQuery
            .OrderByDescending(x => x.EndTime)
            .ToListAsync(ct);

        var passCount  = rows.Count(x => IsPass(x.Status));
        var failCount  = rows.Count(x => !IsPass(x.Status));
        var totalCount = rows.Count;
        var yieldRate  = totalCount > 0
            ? Math.Round(passCount / (double)totalCount * 100, 2)
            : 0.0;

        var details = rows
            .Select(x => new FniYieldDetailDto(
                x.SerialNumber,
                x.StartTime,
                x.EndTime,
                MapStatus(x.Status),
                x.CustomerName,
                x.Family,
                x.StepInstance))
            .ToList();

        _logger.LogInformation(
            "FNI Today DPM loaded for Customer={Customer}, Family={Family}. Pass={PassCount}, Total={TotalCount}",
            request.Customer, request.Family, passCount, totalCount);

        return Result.Success(new FniYieldSummaryDto(passCount, failCount, totalCount, yieldRate, details));
    }

    private static bool IsPass(string? status) =>
        string.Equals((status ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase);

    private static string MapStatus(string? status)
    {
        if (string.Equals((status ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase))
            return "Pass";

        if (string.Equals((status ?? string.Empty).Trim(), "f", StringComparison.OrdinalIgnoreCase))
            return "Fail";

        return string.IsNullOrWhiteSpace(status) ? "Unknown" : status.Trim();
    }
}
