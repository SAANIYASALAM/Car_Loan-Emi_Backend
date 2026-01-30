using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContactMessage?> GetByIdAsync(int messageId)
        {
            return await _context.ContactMessages
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.MessageId == messageId);
        }

        public async Task<List<ContactMessage>> GetAllMessagesAsync(ContactFilterDto? filter = null)
        {
            var query = _context.ContactMessages.Include(c => c.User).AsQueryable();

            if (filter != null)
            {
                if (filter.Status.HasValue)
                    query = query.Where(c => c.Status == filter.Status.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(c => c.CreatedAt >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(c => c.CreatedAt <= filter.ToDate.Value);
            }

            return await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<List<ContactMessage>> GetUserMessagesAsync(int userId)
        {
            return await _context.ContactMessages
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ContactMessage> CreateAsync(ContactMessage message)
        {
            _context.ContactMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<ContactMessage> UpdateAsync(ContactMessage message)
        {
            _context.ContactMessages.Update(message);
            await _context.SaveChangesAsync();
            return message;
        }
    }
}
