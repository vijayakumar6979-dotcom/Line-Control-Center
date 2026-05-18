using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Infrastructure.Data;

public sealed class ManufacturingDbContext : DbContext, IManufacturingDbContext, IUnitOfWork
{
    public ManufacturingDbContext(DbContextOptions<ManufacturingDbContext> options)
        : base(options) { }

    public DbSet<BkFctUph> BkFctUphs { get; set; }
    public DbSet<BkTestTarRawDatum> BkTestTarRawData { get; set; }

    IQueryable<BkFctUph>         IManufacturingDbContext.BkFctUphs       => BkFctUphs;
    IQueryable<BkTestTarRawDatum> IManufacturingDbContext.BkTestTarRawData => BkTestTarRawData;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ManufacturingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}