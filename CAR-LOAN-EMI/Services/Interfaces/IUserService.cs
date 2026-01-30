using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(int userId, UpdateUserDto updateDto);
        Task<ApiResponseDto<object>> GetUserDashboardAsync(int userId);
    }
}
