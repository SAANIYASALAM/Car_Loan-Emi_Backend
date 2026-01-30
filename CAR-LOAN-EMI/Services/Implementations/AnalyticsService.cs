using System.Globalization;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;

        public AnalyticsService(ILoanRepository loanRepository, IUserRepository userRepository)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
        }

        public async Task<ApiResponseDto<MonthlyRevenueDto>> GetMonthlyRevenueAsync(int months = 12)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddMonths(-months);
                var loans = await _loanRepository.GetApprovedLoansSince(startDate);

                var monthlyData = loans
                    .GroupBy(l => new { l.ApplicationDate.Year, l.ApplicationDate.Month })
                    .Select(g => new MonthlyRevenueItemDto
                    {
                        Month = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month)} {g.Key.Year}",
                        TotalDisbursed = g.Sum(l => l.LoanAmount),
                        CarTypeBreakdown = g.GroupBy(l => l.CarType)
                            .Select(ct => new CarTypeBreakdownDto
                            {
                                CarType = ct.Key.ToString(),
                                Amount = ct.Sum(l => l.LoanAmount),
                                Count = ct.Count()
                            }).ToList()
                    })
                    .OrderBy(m => m.Month)
                    .ToList();

                var result = new MonthlyRevenueDto { Data = monthlyData };
                return ApiResponseDto<MonthlyRevenueDto>.SuccessResponse(result, "Monthly revenue retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<MonthlyRevenueDto>.ErrorResponse($"Failed to retrieve monthly revenue: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<CarTypeDistributionDto>> GetCarTypeDistributionAsync()
        {
            try
            {
                var loans = await _loanRepository.GetAllActiveLoans();

                if (loans.Count == 0)
                {
                    return ApiResponseDto<CarTypeDistributionDto>.SuccessResponse(
                        new CarTypeDistributionDto { Distribution = new List<CarTypeDistributionItemDto>() },
                        "No active loans found");
                }

                var distribution = loans
                    .GroupBy(l => l.CarType)
                    .Select(g => new CarTypeDistributionItemDto
                    {
                        CarType = g.Key.ToString(),
                        Count = g.Count(),
                        TotalAmount = g.Sum(l => l.LoanAmount),
                        Percentage = (decimal)g.Count() / loans.Count * 100
                    })
                    .OrderByDescending(d => d.Count)
                    .ToList();

                var result = new CarTypeDistributionDto { Distribution = distribution };
                return ApiResponseDto<CarTypeDistributionDto>.SuccessResponse(result, "Car type distribution retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<CarTypeDistributionDto>.ErrorResponse($"Failed to retrieve car type distribution: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<DashboardStatsDto>> GetDashboardStatsAsync()
        {
            try
            {
                var allLoans = await _loanRepository.GetAllLoansAsync();
                var allUsers = await _userRepository.GetAllUsersAsync();

                var stats = new DashboardStatsDto
                {
                    TotalLoansDisbursed = allLoans
                        .Where(l => l.Status != LoanStatus.Rejected)
                        .Sum(l => l.LoanAmount),
                    ActiveUsers = allUsers.Count(u => u.IsActive),
                    PendingApprovals = allLoans.Count(l => l.Status == LoanStatus.Pending),
                    TotalLoans = allLoans.Count,
                    AverageInterestRate = allLoans.Any() ? allLoans.Average(l => l.InterestRate) : 0,
                    ThisMonthDisbursals = allLoans
                        .Where(l => l.DisbursementDate.HasValue &&
                                   l.DisbursementDate.Value.Month == DateTime.UtcNow.Month &&
                                   l.DisbursementDate.Value.Year == DateTime.UtcNow.Year)
                        .Sum(l => l.LoanAmount)
                };

                return ApiResponseDto<DashboardStatsDto>.SuccessResponse(stats, "Dashboard statistics retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<DashboardStatsDto>.ErrorResponse($"Failed to retrieve dashboard statistics: {ex.Message}");
            }
        }
    }
}
