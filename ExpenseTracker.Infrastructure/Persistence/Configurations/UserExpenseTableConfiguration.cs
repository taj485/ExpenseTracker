using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations
{
    public class UserExpenseTableConfiguration : IEntityTypeConfiguration<UserExpenseTable>
    {
        public void Configure(EntityTypeBuilder<UserExpenseTable> builder)
        {
            builder.ToTable("UserExpenseTables");

            builder.HasKey(m => new { m.UserId, m.ExpenseTableId });

            builder.Property(m => m.IsAdmin)
                .IsRequired();

            builder.HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.ExpenseTable)
                .WithMany(t => t.Members)
                .HasForeignKey(m => m.ExpenseTableId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
