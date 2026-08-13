using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class ReceiptExtractionJobConfiguration : IEntityTypeConfiguration<ReceiptExtractionJob>
    {
        public void Configure(EntityTypeBuilder<ReceiptExtractionJob> builder)
        {
            builder.ToTable("ReceiptExtractionJobs");

            builder.HasKey(j => j.Id);

            // Client-generated: the id is minted before publishing so it can go on the Kafka message.
            builder.Property(j => j.Id)
                .ValueGeneratedNever();

            builder.Property(j => j.UserId)
                .IsRequired();

            builder.Property(j => j.ExpenseTableId)
                .IsRequired();

            builder.Property(j => j.CreatedAtUtc)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();

            builder.HasIndex(j => j.UserId);
        }
    }
}
