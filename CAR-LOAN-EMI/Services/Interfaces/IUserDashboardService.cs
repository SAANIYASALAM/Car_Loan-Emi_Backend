using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IUserDashboardService
    {
        Task<ApiResponseDto<UserDashboardDto>> GetDashboardDataAsync(int userId);
    }
}
