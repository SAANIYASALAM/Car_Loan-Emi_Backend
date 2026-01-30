using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Loans)
                .Include(u => u.KycDocuments)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync(UserFilterDto? filter = null)
        {
            var query = _context.Users.AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.KycStatus))
                    query = query.Where(u => u.KycStatus == filter.KycStatus);

                if (filter.IsActive.HasValue)
                    query = query.Where(u => u.IsActive == filter.IsActive.Value);

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    query = query.Where(u => 
                        u.FullName.ToLower().Contains(searchTerm) || 
                        u.Email.ToLower().Contains(searchTerm));
                }
            }

            return await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }
    }
}
