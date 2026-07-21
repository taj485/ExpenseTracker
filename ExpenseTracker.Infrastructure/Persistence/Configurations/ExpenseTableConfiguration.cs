using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class ExpenseTableConfiguration : IEntityTypeConfiguration<ExpenseTable>
    {
        public void Configure(EntityTypeBuilder<ExpenseTable> builder)
        {
            builder.ToTable("ExpenseTables");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.IsDeleted)
                .IsRequired();

            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(t => t.Members)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
