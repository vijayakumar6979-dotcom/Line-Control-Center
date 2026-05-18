using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.Compliance;

/// <summary>
/// Returns QRQC tickets for an arbitrary date range
/// filtered to customer = 'MSI GP' and process starting with 'HLA'.
/// </summary>
public sealed record GetQrqcTicketsDateRangeQuery(
    DateTime DateFrom,
    DateTime DateTo) : IRequest<Result<IReadOnlyList<LccQrqcTicketDto>>>;

/// <summary>Handles <see cref="GetQrqcTicketsDateRangeQuery"/>.</summary>
public sealed class GetQrqcTicketsDateRangeQueryHandler
    : IRequestHandler<GetQrqcTicketsDateRangeQuery, Result<IReadOnlyList<LccQrqcTicketDto>>>
{
    private readonly IPostgresqlDbContext _db;

    public GetQrqcTicketsDateRangeQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<LccQrqcTicketDto>>> Handle(
        GetQrqcTicketsDateRangeQuery request, CancellationToken ct)
    {
        var results = await _db.LccQrqcTickets
            .AsNoTracking()
            .Where(x => x.Customer == "MSI GP"
                     && x.Process != null && x.Process.StartsWith("HLA")
                     && x.CreationDate >= request.DateFrom
                     && x.CreationDate <= request.DateTo)
            .OrderByDescending(x => x.CreationDate)
            .Select(x => new LccQrqcTicketDto(
                x.Id,
                x.BayNoId,
                x.QrapId,
                x.CreationDate,
                x.TicketId,
                x.Batch,
                x.Bay,
                x.Status,
                x.Customer,
                x.Step,
                x.UpdatedDatetime,
                x.AgingDays,
                x.Process,
                x.Symptom))
            .ToListAsync(ct);

        return Result<IReadOnlyList<LccQrqcTicketDto>>.Success((IReadOnlyList<LccQrqcTicketDto>)results);
    }
}
