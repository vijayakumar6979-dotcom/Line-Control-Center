using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>
/// Returns the Customer/Division/Family group with the most recent activity in mes_main
/// for Motorola (cust_id = 186), along with the end_time date for auto-selecting
/// the shift date on the configuration screen.
/// </summary>
public sealed record GetCurrentRunningFamilyQuery
    : IRequest<Result<(string? Customer, string? Division, string? Family, DateTime? ShiftDate)>>;

public sealed class GetCurrentRunningFamilyQueryHandler
    : IRequestHandler<GetCurrentRunningFamilyQuery, Result<(string? Customer, string? Division, string? Family, DateTime? ShiftDate)>>
{
    private const int MotorolaCustomerId = 186;

    private readonly IPostgresqlDbContext _db;

    public GetCurrentRunningFamilyQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<(string? Customer, string? Division, string? Family, DateTime? ShiftDate)>> Handle(
        GetCurrentRunningFamilyQuery request, CancellationToken ct)
    {
        // Find the most recently active Motorola record in mes_main and resolve the
        // family/customer names from the same reference tables used by the groups list,
        // so the returned strings match exactly and auto-select never silently misses.
        var latest = await (
            from mm in _db.MesMains
            join mf in _db.MesFamilies  on (int)mm.FamilyId!.Value equals mf.FamilyId
            join mc in _db.MesCustomers on mm.CustId              equals (int?)mc.CustomerId
            where mm.CustId    == MotorolaCustomerId
               && mm.FamilyId  != null
               && mm.EndTime   != null
            orderby mm.EndTime descending
            select new
            {
                mc.CustomerName,
                mc.DivisionText,
                mf.Family,
                mm.EndTime
            }
        )
        .AsNoTracking()
        .FirstOrDefaultAsync(ct);

        if (latest is null)
            return Result.Success<(string?, string?, string?, DateTime?)>((null, null, null, null));

        var shiftDate = latest.EndTime!.Value.Date;

        return Result.Success<(string?, string?, string?, DateTime?)>(
            (latest.CustomerName, latest.DivisionText, latest.Family, shiftDate));
    }
}
