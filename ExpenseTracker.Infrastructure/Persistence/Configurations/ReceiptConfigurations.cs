using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class ReceiptConfigurations : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> builder)
        {
            builder.ToTable("Receipts");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UploadedAt)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder.Property(r => r.ImageReference)
                .HasMaxLength(500);

            builder.Navigation(r => r.Expenses)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
