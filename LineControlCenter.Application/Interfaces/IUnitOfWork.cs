namespace LineControlCenter.Application.Interfaces;

/// <summary>
/// Abstracts the persistence commit operation so command handlers can save
/// without a direct dependency on EF Core or any DbContext.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists all pending changes in the current unit of work.</summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
