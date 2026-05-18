using LineControlCenter.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Infrastructure.Data;

public sealed class PostgresqlDbContextFactory : IPostgresqlDbContextFactory
{
    private readonly IDbContextFactory<PostgresqlDbContext> _factory;

    public PostgresqlDbContextFactory(IDbContextFactory<PostgresqlDbContext> factory)
        => _factory = factory;

    public async Task<IPostgresqlDbContext> CreateDbContextAsync(CancellationToken ct = default)
        => await _factory.CreateDbContextAsync(ct);
}
