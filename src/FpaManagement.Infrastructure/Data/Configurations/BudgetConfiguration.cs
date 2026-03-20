using FpaManagement.Domain.Entities;
using FpaManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FpaManagement.Infrastructure.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Description)
            .HasMaxLength(1000);

        builder.Property(b => b.FiscalYear)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.OwnsOne(b => b.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasDefaultValue("BRL");
        });

        builder.HasIndex(b => b.FiscalYear);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => new { b.FiscalYear, b.Status });

        builder.HasOne(b => b.Department)
            .WithMany()
            .HasForeignKey(b => b.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.CostCenter)
            .WithMany()
            .HasForeignKey(b => b.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(b => !b.IsDeleted);
    }
}
