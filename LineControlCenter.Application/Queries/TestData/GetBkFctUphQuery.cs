using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.TestData;

/// <summary>Returns filtered BK FCT UPH records.</summary>
public sealed record GetBkFctUphQuery(
    string? Customer,
    string? Family,
    string? TestStatus,
    string? Shift,
    string? ShiftDate) : IRequest<Result<IReadOnlyList<BkFctUphDto>>>;

/// <summary>Handles <see cref="GetBkFctUphQuery"/>.</summary>
public sealed class GetBkFctUphQueryHandler
    : IRequestHandler<GetBkFctUphQuery, Result<IReadOnlyList<BkFctUphDto>>>
{
    private readonly IManufacturingDbContext _db;

    public GetBkFctUphQueryHandler(IManufacturingDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<BkFctUphDto>>> Handle(
        GetBkFctUphQuery request, CancellationToken ct)
    {
        var query = _db.BkFctUphs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(request.Customer))
            query = query.Where(x => x.Customer == request.Customer);

        if (!string.IsNullOrEmpty(request.Family))
            query = query.Where(x => x.Family == request.Family);

        if (!string.IsNullOrEmpty(request.TestStatus))
            query = query.Where(x => x.TestStatus == request.TestStatus);

        if (!string.IsNullOrEmpty(request.Shift))
            query = query.Where(x => x.Shift == request.Shift);

        if (!string.IsNullOrEmpty(request.ShiftDate))
            query = query.Where(x => x.ShiftDate == request.ShiftDate);

        var results = await query
            .Select(x => new BkFctUphDto(
                x.SerialNumber, x.Number, x.Revision, x.Customer,
                x.Division, x.Family, x.TestFactory, x.TestRoute,
                x.TestRouteStep, x.TestEquipment,
                x.TestStartDateTime, x.TestEndDateTime,
                x.TestStatus, x.ProcessLoop, x.TestLoop,
                x.TestUser, x.Type, x.Shift, x.ShiftDate, x.TimeRange))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<BkFctUphDto>>(results);
    }
}
