using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LineControlCenter.Infrastructure.Data.Configurations;

/// <summary>EF Core entity configuration for <see cref="BkTestTarRawDatum"/>.</summary>
internal sealed class BkTestTarRawDatumConfiguration : IEntityTypeConfiguration<BkTestTarRawDatum>
{
    public void Configure(EntityTypeBuilder<BkTestTarRawDatum> builder)
    {
        builder.HasNoKey();
        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.DomainEvents);
        builder.ToTable("BK_Test_Tar_RawData", "db_owner");

        builder.Property(e => e.SerialNumber).HasColumnName("SerialNumber").HasMaxLength(25);
        builder.Property(e => e.Customer).HasColumnName("Customer").HasMaxLength(20);
        builder.Property(e => e.Division).HasColumnName("Division").HasMaxLength(25);
        builder.Property(e => e.Family).HasColumnName("Family");
        builder.Property(e => e.Number).HasColumnName("Number").HasMaxLength(25);
        builder.Property(e => e.Process).HasColumnName("Process").HasMaxLength(50);
        builder.Property(e => e.TestStatus).HasColumnName("TestStatus").HasMaxLength(1).IsFixedLength();
        builder.Property(e => e.StartDateTime).HasColumnName("StartDateTime").HasColumnType("datetime2");
        builder.Property(e => e.EndDateTime).HasColumnName("EndDateTime").HasColumnType("datetime2");
        builder.Property(e => e.Operator).HasColumnName("Operator").HasMaxLength(35);
        builder.Property(e => e.TestFailure).HasColumnName("TestFailure");
        builder.Property(e => e.Rmastatus).HasColumnName("RMAStatus").HasMaxLength(5);
        builder.Property(e => e.TestLoopCount).HasColumnName("TestLoopCount");
        builder.Property(e => e.TesterName).HasColumnName("TesterName").HasMaxLength(45);
        builder.Property(e => e.Source).HasColumnName("Source").HasMaxLength(7).IsFixedLength();
        builder.Property(e => e.Shift).HasColumnName("Shift").HasMaxLength(7);
        builder.Property(e => e.ShiftDate).HasColumnName("ShiftDate").HasMaxLength(10);
        builder.Property(e => e.TimeRange).HasColumnName("TimeRange").HasMaxLength(5);
    }
}
