namespace LineControlCenter.Application.Interfaces;

/// <summary>Creates short-lived <see cref="IPostgresqlDbContext"/> instances safe for concurrent use.</summary>
public interface IPostgresqlDbContextFactory
{
    /// <summary>Creates and returns a new <see cref="IPostgresqlDbContext"/> instance.</summary>
    Task<IPostgresqlDbContext> CreateDbContextAsync(CancellationToken ct = default);
}
