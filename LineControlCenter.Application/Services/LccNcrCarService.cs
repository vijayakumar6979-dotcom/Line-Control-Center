using LineControlCenter.Application.DTOs;
using LineControlCenter.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Services;

/// <summary>Reads NCR/CAR records from PostgreSQL.</summary>
public sealed class LccNcrCarService : ILccNcrCarService
{
    private readonly IPostgresqlDbContext _db;

    public LccNcrCarService(IPostgresqlDbContext db) => _db = db;

    public async Task<IReadOnlyList<LccNcrcarsTblDto>> GetByBayAsync(
        string bay, CancellationToken ct = default)
    {
        return await _db.LccNcrCarsTbls
            .AsNoTracking()
            .Where(x => x.Bay == bay)
            .OrderByDescending(x => x.IssueDate)
            .Select(x => new LccNcrcarsTblDto(
                x.NcrcarNoId, x.NcrcarNo, x.NcrType, x.Status, x.CarOwner,
                x.AcknowledgeDate, x.Plant, x.Customer, x.Department,
                x.ProblemStatementCategory, x.ProblemStatement, x.ProblemDescription,
                x.Bay, x.StationArea, x.IssueBy, x.IssueDate,
                x.ResponseStatus, x.RespondedAging,
                x.ClosureDate, x.ClosureStatus, x.ClosureAging))
            .ToListAsync(ct);
    }
}
