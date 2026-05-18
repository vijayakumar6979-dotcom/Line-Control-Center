using LineControlCenter.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LineControlCenter.Infrastructure.Data.Configurations;

/// <summary>EF Core entity configuration for <see cref="BkFctUph"/>.</summary>
internal sealed class BkFctUphConfiguration : IEntityTypeConfiguration<BkFctUph>
{
    public void Configure(EntityTypeBuilder<BkFctUph> builder)
    {
        builder.HasNoKey();
        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.DomainEvents);
        builder.ToTable("BK_FCT_UPH", "db_owner");

        builder.Property(e => e.SerialNumber).HasColumnName("SerialNumber").HasMaxLength(100);
        builder.Property(e => e.Number).HasColumnName("Number").HasMaxLength(100);
        builder.Property(e => e.Revision).HasColumnName("Revision").HasMaxLength(20);
        builder.Property(e => e.Customer).HasColumnName("Customer").HasMaxLength(50);
        builder.Property(e => e.Division).HasColumnName("Division").HasMaxLength(50);
        builder.Property(e => e.Family).HasColumnName("Family").HasMaxLength(100);
        builder.Property(e => e.TestFactory).HasColumnName("TestFactory").HasMaxLength(100);
        builder.Property(e => e.TestRoute).HasColumnName("TestRoute").HasMaxLength(100);
        builder.Property(e => e.TestRouteStep).HasColumnName("TestRouteStep").HasMaxLength(100);
        builder.Property(e => e.TestEquipment).HasColumnName("TestEquipment").HasMaxLength(300);
        builder.Property(e => e.TestStartDateTime).HasColumnName("TestStartDateTime").HasColumnType("datetime");
        builder.Property(e => e.TestEndDateTime).HasColumnName("TestEndDateTime").HasColumnType("datetime");
        builder.Property(e => e.TestStatus).HasColumnName("TestStatus").HasMaxLength(10);
        builder.Property(e => e.ProcessLoop).HasColumnName("ProcessLoop").HasMaxLength(5);
        builder.Property(e => e.TestLoop).HasColumnName("TestLoop").HasMaxLength(5);
        builder.Property(e => e.TestUserIdId).HasColumnName("TestUserID_ID").HasMaxLength(100);
        builder.Property(e => e.TestUser).HasColumnName("TestUser").HasMaxLength(100);
        builder.Property(e => e.Type).HasColumnName("Type").HasMaxLength(10);
        builder.Property(e => e.Shift).HasColumnName("Shift").HasMaxLength(10);
        builder.Property(e => e.ShiftDate).HasColumnName("ShiftDate").HasMaxLength(15);
        builder.Property(e => e.TimeRange).HasColumnName("TimeRange").HasMaxLength(20);
    }
}
