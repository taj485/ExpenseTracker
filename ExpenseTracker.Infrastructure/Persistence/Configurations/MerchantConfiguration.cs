using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchants");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.NormalizedName)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(m => m.NormalizedName)
                .IsUnique();

            // Longest possible DNS name.
            builder.Property(m => m.Website)
                .HasMaxLength(253);

            builder.HasMany(m => m.Aliases)
                   .WithOne(a => a.Merchant)
                   .HasForeignKey(a => a.MerchantId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(MerchantSeedData.Merchants);
        }
    }
}
