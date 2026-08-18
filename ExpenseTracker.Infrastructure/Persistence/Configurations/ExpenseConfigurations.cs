using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenseTracker.Domain.ValueObjects;
using System;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class ExpenseConfigurations : IEntityTypeConfiguration<Expense>
    {
        public void Configure(EntityTypeBuilder<Expense> builder)
        {
            builder.ToTable("Expenses");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.ComplexProperty(e => e.UnitPrice, money =>
            {
                money.Property(e => e.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
            });

            builder.Property(e => e.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(e => e.Date)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder.Property(e => e.Category)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.IsDeleted)
                .IsRequired();

            builder.Property(e => e.Merchant)
                .HasMaxLength(200);

            builder.HasQueryFilter(e => !e.IsDeleted);

            builder.HasOne(e => e.Receipt)
                   .WithMany(r => r.Expenses)
                   .HasForeignKey(e => e.ReceiptId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(e => e.ExpenseTable)
                   .WithMany()
                   .HasForeignKey(e => e.ExpenseTableId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
