using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class DeletedAccountConfiguration : IEntityTypeConfiguration<DeletedAccount>
    {
        public void Configure(EntityTypeBuilder<DeletedAccount> builder)
        {
            builder.ToTable("DeletedAccounts");

            builder.HasKey(d => d.Subject);

            builder.Property(d => d.Subject)
                .HasMaxLength(255);

            builder.Property(d => d.DeletedAt)
                .HasConversion(
                    v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                    v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .IsRequired();
        }
    }
}
