using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Application.Settings;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>
/// Returns FNI yield data for the current calendar day (today 00:00 → now),
/// using the same MES joins as <see cref="GetFniYieldWtdQuery"/> but scoped to today only.
/// </summary>
public sealed record GetFniYieldTodayQuery(
    string Customer,
    string Family) : IRequest<Result<FniYieldSummaryDto>>;

public sealed class GetFniYieldTodayQueryHandler
    : IRequestHandler<GetFniYieldTodayQuery, Result<FniYieldSummaryDto>>
{
    private readonly IPostgresqlDbContext _db;
    private readonly ILogger<GetFniYieldTodayQueryHandler> _logger;
    private readonly IOptionsMonitor<DpmSettings> _dpmOptions;

    public GetFniYieldTodayQueryHandler(
        IPostgresqlDbContext db,
        ILogger<GetFniYieldTodayQueryHandler> logger,
        IOptionsMonitor<DpmSettings> dpmOptions)
    {
        _db         = db;
        _logger     = logger;
        _dpmOptions = dpmOptions;
    }

    public async Task<Result<FniYieldSummaryDto>> Handle(
        GetFniYieldTodayQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Customer))
            return Result.Failure<FniYieldSummaryDto>(
                new Error("FNI.CustomerRequired", "Customer is required for FNI yield query."));

        if (string.IsNullOrWhiteSpace(request.Family))
            return Result.Failure<FniYieldSummaryDto>(
                new Error("FNI.FamilyRequired", "Family is required for FNI yield query."));

        // end_time is "timestamp WITHOUT time zone" — must use Unspecified Kind so
        // Npgsql sends wall-clock values as-is instead of converting Local→UTC.
        var todayStart = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Unspecified);
        var now        = DateTime.SpecifyKind(DateTime.Now,   DateTimeKind.Unspecified);

        var settings   = _dpmOptions.CurrentValue;
        var fniSteps   = settings.FniStepNames;
        var filterProc = settings.FilterProcessLoop;
        var filterTest = settings.FilterTestLoop;

        var familyIds = await _db.MesFamilies
            .AsNoTracking()
            .Where(f => f.Family == request.Family)
            .Select(f => (long)f.FamilyId)
            .Distinct()
            .ToListAsync(ct);

        if (familyIds.Count == 0)
            return Result.Success(new FniYieldSummaryDto(0, 0, 0, 0, []));

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
                  && rs.CustId    == m.CustId
                  && rs.CustId    == (int?)c.CustomerId
                  && fniSteps.Contains(rs.StepInstance ?? "")
                  && (!filterTest || m.TestLoop == 1)
                  && (!filterProc || m.ProcessLoop == 1)
                  && familyIds.Contains(m.FamilyId!.Value)
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
            "FNI Today yield loaded for Customer={Customer}, Family={Family}. Pass={PassCount}, Total={TotalCount}, Yield={YieldRate}",
            request.Customer, request.Family, passCount, totalCount, yieldRate);

        return Result.Success(new FniYieldSummaryDto(passCount, failCount, totalCount, yieldRate, details));
    }

    private static bool IsPass(string? status) =>
        string.Equals((status ?? string.Empty).Trim(), "p", StringComparison.OrdinalIgnoreCase);

    private static string MapStatus(string? status) =>
        IsPass(status) ? "Pass" : "Fail";
}
