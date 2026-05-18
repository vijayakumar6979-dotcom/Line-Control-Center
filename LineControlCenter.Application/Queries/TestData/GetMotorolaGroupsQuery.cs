using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns distinct Customer/Division/Family rows from engdf_db MES tables, filtered to Motorola (customer_id = 186).</summary>
public sealed record GetMotorolaGroupsQuery : IRequest<Result<IReadOnlyList<MesGroupItemDto>>>;

/// <summary>DTO returned by <see cref="GetMotorolaGroupsQuery"/>.</summary>
public sealed record MesGroupItemDto(
    string CustomerName,
    string? DivisionText,
    string? Family);

public sealed class GetMotorolaGroupsQueryHandler
    : IRequestHandler<GetMotorolaGroupsQuery, Result<IReadOnlyList<MesGroupItemDto>>>
{
    private const int MotorolaCustomerId = 186;

    private readonly IPostgresqlDbContext _db;

    public GetMotorolaGroupsQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<MesGroupItemDto>>> Handle(
        GetMotorolaGroupsQuery request, CancellationToken ct)
    {
        // Step 1 — tiny reference table: get Motorola customer display columns.
        var customers = await _db.MesCustomers
            .AsNoTracking()
            .Where(c => c.CustomerId == MotorolaCustomerId)
            .Select(c => new { c.CustomerName, c.DivisionText })
            .Distinct()
            .ToListAsync(ct);

        if (customers.Count == 0)
            return Result.Success<IReadOnlyList<MesGroupItemDto>>(Array.Empty<MesGroupItemDto>());

        // Step 2 — loose-index-scan CTE via infrastructure method.
        // mes_main has 29M+ Motorola rows but only ~17 distinct family_ids.
        // This makes exactly 17 index seeks instead of scanning 29M rows.
        var familyIds = await _db.GetDistinctFamilyIdsForCustomerAsync(MotorolaCustomerId, ct);

        if (familyIds.Count == 0)
            return Result.Success<IReadOnlyList<MesGroupItemDto>>(Array.Empty<MesGroupItemDto>());

        // Step 3 — PK lookup on the tiny mes_family reference table.
        var familyIdInts = familyIds.Select(id => (int)id).ToList();
        var families = await _db.MesFamilies
            .AsNoTracking()
            .Where(f => familyIdInts.Contains(f.FamilyId))
            .Select(f => f.Family!)
            .Where(f => f != null)
            .OrderBy(f => f)
            .ToListAsync(ct);

        // Step 4 — in-memory cross-join: handful of customers × ~17 families.
        var results = customers
            .SelectMany(c => families, (c, f) => new MesGroupItemDto(
                c.CustomerName ?? string.Empty,
                c.DivisionText,
                f))
            .OrderBy(x => x.CustomerName)
            .ThenBy(x => x.Family)
            .DistinctBy(x => $"{x.CustomerName}|{x.DivisionText}|{x.Family}")
            .ToList();

        return Result.Success<IReadOnlyList<MesGroupItemDto>>(results);
    }
}
