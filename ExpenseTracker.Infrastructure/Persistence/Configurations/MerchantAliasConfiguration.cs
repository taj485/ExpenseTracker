using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class MerchantAliasConfiguration : IEntityTypeConfiguration<MerchantAlias>
    {
        public void Configure(EntityTypeBuilder<MerchantAlias> builder)
        {
            builder.ToTable("MerchantAliases");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.NormalizedAlias)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(a => a.NormalizedAlias)
                .IsUnique();

            builder.HasData(MerchantSeedData.Aliases);
        }
    }
}
