using FpaManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FpaManagement.Infrastructure.Data.Configurations;

public class BudgetVersionConfiguration : IEntityTypeConfiguration<BudgetVersion>
{
    public void Configure(EntityTypeBuilder<BudgetVersion> builder)
    {
        builder.ToTable("BudgetVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VersionName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.Description)
            .HasMaxLength(500);

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.IsCurrent)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(v => new { v.BudgetId, v.VersionNumber })
            .IsUnique();

        builder.HasOne(v => v.Budget)
            .WithMany(b => b.Versions)
            .HasForeignKey(v => v.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
