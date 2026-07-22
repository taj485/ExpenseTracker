using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ExpenseTracker.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserReader, IUserWriter
    {
        private readonly ExpenseTrackerDbContext _context;

        public UserRepository(ExpenseTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUserIdpAsync(string subject, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Subject == subject, ct);
        }

        public async Task<IReadOnlyList<User>> GetAllByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .ToListAsync(ct);
        }

        public async Task<int> AddAsync(User user, CancellationToken ct)
        {
            await _context.Users.AddAsync(user, ct);
            await SaveChangesAsync(ct);
            return user.Id;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}