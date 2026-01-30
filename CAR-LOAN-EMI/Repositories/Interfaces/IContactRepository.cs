using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface IContactRepository
    {
        Task<ContactMessage?> GetByIdAsync(int messageId);
        Task<List<ContactMessage>> GetAllMessagesAsync(ContactFilterDto? filter = null);
        Task<List<ContactMessage>> GetUserMessagesAsync(int userId);
        Task<ContactMessage> CreateAsync(ContactMessage message);
        Task<ContactMessage> UpdateAsync(ContactMessage message);
    }
}
