using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns one representative record per distinct Customer/Division/Family group.</summary>
public sealed record GetDistinctGroupsQuery : IRequest<Result<IReadOnlyList<BkTestTarRawDatumDto>>>;

/// <summary>Handles <see cref="GetDistinctGroupsQuery"/>.</summary>
public sealed class GetDistinctGroupsQueryHandler
    : IRequestHandler<GetDistinctGroupsQuery, Result<IReadOnlyList<BkTestTarRawDatumDto>>>
{
    private readonly IJbkTeDbContext _db;

    public GetDistinctGroupsQueryHandler(IJbkTeDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<BkTestTarRawDatumDto>>> Handle(
        GetDistinctGroupsQuery request, CancellationToken ct)
    {
        var results = await _db.BkUphTars
            .AsNoTracking()
            .Where(x => x.Customer != null && x.Division != null && x.Family != null)
            .Select(x => new { x.Customer, x.Division, x.Family })
            .Distinct()
            .OrderBy(x => x.Customer)
            .ThenBy(x => x.Family)
            .Select(x => new BkTestTarRawDatumDto(
                string.Empty, x.Customer, x.Division, x.Family,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<BkTestTarRawDatumDto>>(results);
    }
}

