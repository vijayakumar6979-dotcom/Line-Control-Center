using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain.Safety;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Application.Services;

/// <summary>Reads safety incident records from PostgreSQL.</summary>
public sealed class LccSafetyTblService : ILccSafetyTblService
{
    private readonly IPostgresqlDbContext _db;

    public LccSafetyTblService(IPostgresqlDbContext db) => _db = db;

    public async Task<IEnumerable<LccSafetyTbl>> GetByFilterAsync(
        DateTime? fromDate, DateTime? toDate, CancellationToken ct = default)
    {
        var query = _db.LccSafetyTbls.AsNoTracking().AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(x => x.IncidentDatetime >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x => x.IncidentDatetime <= toDate.Value);

        return await query.ToListAsync(ct);
    }
}
