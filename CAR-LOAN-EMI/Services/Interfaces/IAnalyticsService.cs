using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<ApiResponseDto<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 12);
        Task<ApiResponseDto<CarTypeDistributionDto>> GetCarTypeDistributionAsync();
        Task<ApiResponseDto<DashboardStatsDto>> GetDashboardStatsAsync();
    }
}
