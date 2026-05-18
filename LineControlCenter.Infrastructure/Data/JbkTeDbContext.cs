using LineControlCenter.Application.Interfaces;
using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;

namespace LineControlCenter.Infrastructure.Data;

/// <summary>EF Core DbContext for the jbk_te PostgreSQL database (public.bk_uph_tar).</summary>
public sealed class JbkTeDbContext : DbContext, IJbkTeDbContext
{
    public JbkTeDbContext(DbContextOptions<JbkTeDbContext> options) : base(options) { }

    public DbSet<BkUphTar> BkUphTars { get; set; }

    IQueryable<BkUphTar> IJbkTeDbContext.BkUphTars => BkUphTars;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BkUphTar>(entity =>
        {
            entity.HasNoKey()
                  .ToTable("bk_uph_tar", "public");

            entity.Property(e => e.SerialNumber).HasColumnName("serialnumber").HasMaxLength(50);
            entity.Property(e => e.Customer).HasColumnName("customer").HasMaxLength(50);
            entity.Property(e => e.Division).HasColumnName("division").HasMaxLength(50);
            entity.Property(e => e.Family).HasColumnName("family").HasMaxLength(100);
            entity.Property(e => e.Number).HasColumnName("number").HasMaxLength(50);
            entity.Property(e => e.Process).HasColumnName("process").HasMaxLength(50);
            entity.Property(e => e.TestStatus).HasColumnName("teststatus").HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.StartDateTime).HasColumnName("startdatetime");
            entity.Property(e => e.EndDateTime).HasColumnName("enddatetime");
            entity.Property(e => e.Operator).HasColumnName("operator");
            entity.Property(e => e.TestFailure).HasColumnName("testfailure");
            entity.Property(e => e.RmaStatus).HasColumnName("rmastatus").HasMaxLength(5);
            entity.Property(e => e.TestLoopCount).HasColumnName("testloopcount").HasMaxLength(5);
            entity.Property(e => e.TesterName).HasColumnName("testername");
            entity.Property(e => e.Source).HasColumnName("source").HasMaxLength(7).IsFixedLength();
            entity.Property(e => e.Shift).HasColumnName("shift").HasMaxLength(7);
            entity.Property(e => e.ShiftDate).HasColumnName("shiftdate").HasMaxLength(10);
            entity.Property(e => e.TimeRange).HasColumnName("timerange").HasMaxLength(5);
        });

        base.OnModelCreating(modelBuilder);
    }
}
