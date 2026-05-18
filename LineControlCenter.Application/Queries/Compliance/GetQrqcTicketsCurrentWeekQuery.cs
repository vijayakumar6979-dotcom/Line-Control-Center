using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.Compliance;

/// <summary>
/// Returns QRQC tickets for the current ISO week (Monday–Sunday)
/// filtered to customer = 'MSI GP' and process starting with 'HLA'.
/// </summary>
public sealed record GetQrqcTicketsCurrentWeekQuery : IRequest<Result<IReadOnlyList<LccQrqcTicketDto>>>;

/// <summary>Handles <see cref="GetQrqcTicketsCurrentWeekQuery"/>.</summary>
public sealed class GetQrqcTicketsCurrentWeekQueryHandler
    : IRequestHandler<GetQrqcTicketsCurrentWeekQuery, Result<IReadOnlyList<LccQrqcTicketDto>>>
{
    private readonly IPostgresqlDbContext _db;

    public GetQrqcTicketsCurrentWeekQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<LccQrqcTicketDto>>> Handle(
        GetQrqcTicketsCurrentWeekQuery request, CancellationToken ct)
    {
        // Calculate Monday 00:00 and Sunday 23:59:59 of the current week
        var today     = DateTime.Today;
        int diff      = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var weekStart = today.AddDays(-diff);
        var weekEnd   = weekStart.AddDays(7).AddSeconds(-1);

        var results = await _db.LccQrqcTickets
            .AsNoTracking()
            .Where(x => x.Customer == "MSI GP"
                     && x.Process != null && x.Process.StartsWith("HLA")
                     && x.CreationDate >= weekStart
                     && x.CreationDate <= weekEnd)
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
