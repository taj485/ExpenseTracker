using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Subject)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Subject)
                .IsUnique();

            builder.Property(u => u.Email)
                .HasMaxLength(320);

            builder.Navigation(u => u.Expenses)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}