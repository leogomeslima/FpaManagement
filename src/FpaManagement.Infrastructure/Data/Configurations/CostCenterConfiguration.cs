using FpaManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FpaManagement.Infrastructure.Data.Configurations;

public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.ToTable("CostCenters");

        builder.HasKey(cc => cc.Id);

        builder.Property(cc => cc.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(cc => cc.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cc => cc.Description)
            .HasMaxLength(500);

        builder.Property(cc => cc.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(cc => cc.AnnualBudget)
            .HasPrecision(18, 2);

        builder.HasIndex(cc => cc.Code)
            .IsUnique();

        builder.HasIndex(cc => cc.Name);

        builder.HasOne(cc => cc.Department)
            .WithMany(d => d.CostCenters)
            .HasForeignKey(cc => cc.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(cc => cc.Manager)
            .WithMany()
            .HasForeignKey(cc => cc.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(cc => !cc.IsDeleted);
    }
}
