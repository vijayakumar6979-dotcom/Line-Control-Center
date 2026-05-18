using LineControlCenter.Application.DTOs;

namespace LineControlCenter.Application.Interfaces;

/// <summary>Reads NCR/CAR records from the PostgreSQL database.</summary>
public interface ILccNcrCarService
{
    /// <summary>Returns all NCR/CAR records for the given bay, ordered by issue date descending.</summary>
    Task<IReadOnlyList<LccNcrcarsTblDto>> GetByBayAsync(string bay, CancellationToken ct = default);
}
