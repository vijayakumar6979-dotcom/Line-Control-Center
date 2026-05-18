using LineControlCenter.Domain.Safety;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Reads safety incident records from the PostgreSQL database.</summary>
public interface ILccSafetyTblService
{
    /// <summary>Returns incidents filtered by optional date range.</summary>
    Task<IEnumerable<LccSafetyTbl>> GetByFilterAsync(
        DateTime? fromDate, DateTime? toDate, CancellationToken ct = default);
}
