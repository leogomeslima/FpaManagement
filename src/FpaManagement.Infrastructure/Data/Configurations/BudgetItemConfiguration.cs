using FpaManagement.Domain.Entities;
using FpaManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FpaManagement.Infrastructure.Data.Configurations;

public class BudgetItemConfiguration : IEntityTypeConfiguration<BudgetItem>
{
    public void Configure(EntityTypeBuilder<BudgetItem> builder)
    {
        builder.ToTable("BudgetItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.SubCategory)
            .HasMaxLength(100);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(i => i.AccountCode)
            .HasMaxLength(50);

        builder.OwnsOne(i => i.PlannedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PlannedAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("BRL");
        });

        builder.OwnsOne(i => i.ActualAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("ActualAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("ActualCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(i => i.Variance, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Variance")
                .HasPrecision(18, 2);
        });

        builder.Property(i => i.VariancePercentage)
            .HasPrecision(18, 2);

        builder.HasIndex(i => i.Category);
        builder.HasIndex(i => new { i.BudgetVersionId, i.Category });

        builder.HasOne(i => i.BudgetVersion)
            .WithMany(v => v.Items)
            .HasForeignKey(i => i.BudgetVersionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.CostCenter)
            .WithMany(cc => cc.BudgetItems)
            .HasForeignKey(i => i.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(i => !i.IsDeleted);
    }
}
