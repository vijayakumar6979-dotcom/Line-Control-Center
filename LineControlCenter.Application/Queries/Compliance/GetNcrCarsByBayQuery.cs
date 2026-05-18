using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Queries.Compliance;

/// <summary>Returns NCR/CAR records for a given bay, ordered by issue date descending.</summary>
public sealed record GetNcrCarsByBayQuery(string Bay)
    : IRequest<Result<IReadOnlyList<LccNcrcarsTblDto>>>;

/// <summary>Handles <see cref="GetNcrCarsByBayQuery"/>.</summary>
public sealed class GetNcrCarsByBayQueryHandler
    : IRequestHandler<GetNcrCarsByBayQuery, Result<IReadOnlyList<LccNcrcarsTblDto>>>
{
    private readonly IPostgresqlDbContext _db;

    public GetNcrCarsByBayQueryHandler(IPostgresqlDbContext db) => _db = db;

    public async Task<Result<IReadOnlyList<LccNcrcarsTblDto>>> Handle(
        GetNcrCarsByBayQuery request, CancellationToken ct)
    {
        // Filter to current calendar week (Monday–Sunday)
        var today = DateTime.Today;
        int diff = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Monday) % 7;
        var weekStart = today.AddDays(-1 * diff).Date;
        var weekEnd = weekStart.AddDays(7).Date;

        var results = await _db.LccNcrCarsTbls
            .AsNoTracking()
            .Where(x => x.Bay == request.Bay && x.IssueDate >= weekStart && x.IssueDate < weekEnd)
            .OrderByDescending(x => x.IssueDate)
            .Select(x => new LccNcrcarsTblDto(
                x.NcrcarNoId, x.NcrcarNo, x.NcrType, x.Status, x.CarOwner,
                x.AcknowledgeDate, x.Plant, x.Customer, x.Department,
                x.ProblemStatementCategory, x.ProblemStatement, x.ProblemDescription,
                x.Bay, x.StationArea, x.IssueBy, x.IssueDate,
                x.ResponseStatus, x.RespondedAging,
                x.ClosureDate, x.ClosureStatus, x.ClosureAging))
            .ToListAsync(ct);

        return Result.Success<IReadOnlyList<LccNcrcarsTblDto>>(results);
    }
}
