using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        
        // New methods for admin functionality
        Task<List<User>> GetAllUsersAsync(UserFilterDto? filter = null);
    }
}
