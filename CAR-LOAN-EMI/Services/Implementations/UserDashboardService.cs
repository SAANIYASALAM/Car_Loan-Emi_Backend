using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IEmiRepository _emiRepository;

        public UserDashboardService(
            IUserRepository userRepository,
            ILoanRepository loanRepository,
            IEmiRepository emiRepository)
        {
            _userRepository = userRepository;
            _loanRepository = loanRepository;
            _emiRepository = emiRepository;
        }

        public async Task<ApiResponseDto<UserDashboardDto>> GetDashboardDataAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponseDto<UserDashboardDto>.ErrorResponse("User not found");

                var loans = await _loanRepository.GetLoansByUserIdAsync(userId);
                
                var preApprovedLimit = CalculatePreApprovedLimit(user);
                var healthScore = await CalculateLoanHealthScore(userId, loans);
                var offers = GeneratePersonalizedOffers(user, preApprovedLimit);
                
                var activeLoans = loans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved);
                var totalPaid = loans.Sum(l => l.PaidAmount);
                var totalRemaining = loans
                    .Where(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved)
                    .Sum(l => l.LoanAmount - l.PaidAmount);

                var dashboard = new UserDashboardDto
                {
                    PreApprovedLimit = preApprovedLimit,
                    CreditScore = user.CreditScore,
                    LoanHealthScore = healthScore,
                    ActiveLoans = activeLoans,
                    TotalPaid = totalPaid,
                    TotalRemaining = totalRemaining,
                    PersonalizedOffers = offers
                };

                return ApiResponseDto<UserDashboardDto>.SuccessResponse(dashboard, "Dashboard data retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<UserDashboardDto>.ErrorResponse($"Failed to retrieve dashboard data: {ex.Message}");
            }
        }

        private decimal CalculatePreApprovedLimit(Models.Entities.User user)
        {
            // Base calculation: 40% of annual income
            var baseLimit = user.MonthlyIncome * 12 * 0.40m;

            // Credit score multiplier
            var creditScoreMultiplier = user.CreditScore switch
            {
                >= 800 => 1.5m,
                >= 750 => 1.3m,
                >= 700 => 1.1m,
                >= 650 => 1.0m,
                _ => 0.8m
            };

            return baseLimit * creditScoreMultiplier;
        }

        private async Task<int> CalculateLoanHealthScore(int userId, List<Models.Entities.Loan> loans)
        {
            if (!loans.Any()) return 100;

            var totalLoanAmount = loans.Sum(l => l.LoanAmount);
            var totalPaid = loans.Sum(l => l.PaidAmount);
            var activeLoans = loans.Count(l => l.Status == LoanStatus.Active);

            // Calculate payment ratio (0-40 points)
            var paymentRatio = totalLoanAmount > 0 ? (totalPaid / totalLoanAmount) * 100 : 100;
            var paymentScore = (paymentRatio / 100) * 40;

            // On-time payment score (0-40 points)
            var onTimeScore = await GetOnTimePaymentScore(userId);

            // Active loans penalty (0-20 points)
            var activeLoanScore = activeLoans switch
            {
                0 => 20,
                1 => 20,
                2 => 15,
                3 => 10,
                _ => 5
            };

            return (int)(paymentScore + onTimeScore + activeLoanScore);
        }

        private async Task<decimal> GetOnTimePaymentScore(int userId)
        {
            var payments = await _emiRepository.GetHistoryByUserIdAsync(userId);
            if (!payments.Any()) return 40;

            var paidPayments = payments.Where(p => p.Status == PaymentStatus.Paid).ToList();
            if (!paidPayments.Any()) return 0;

            // Calculate percentage of on-time payments
            var onTimePayments = paidPayments.Count(p => p.PaymentDate <= p.DueDate);
            var onTimePercentage = (decimal)onTimePayments / paidPayments.Count;

            return onTimePercentage * 40;
        }

        private List<PersonalizedOfferDto> GeneratePersonalizedOffers(Models.Entities.User user, decimal preApprovedLimit)
        {
            var offers = new List<PersonalizedOfferDto>();

            // Credit score based offers
            if (user.CreditScore >= 750)
            {
                offers.Add(new PersonalizedOfferDto
                {
                    Title = "Premium Rate Offer",
                    Description = $"Get special interest rates starting from 8.5% on your next car loan up to ₹{preApprovedLimit:N0}",
                    OfferType = "Interest Rate",
                    ValidUntil = DateTime.UtcNow.AddDays(30)
                });
            }

            // KYC verified offers
            if (user.KycStatus == "Verified")
            {
                offers.Add(new PersonalizedOfferDto
                {
                    Title = "Quick Approval",
                    Description = "As a KYC verified customer, get loan approval within 24 hours",
                    OfferType = "Processing",
                    ValidUntil = DateTime.UtcNow.AddDays(60)
                });
            }

            // High income offers
            if (user.MonthlyIncome >= 100000)
            {
                offers.Add(new PersonalizedOfferDto
                {
                    Title = "Elite Customer Benefits",
                    Description = "Enjoy zero processing fees and flexible repayment options",
                    OfferType = "Fees",
                    ValidUntil = DateTime.UtcNow.AddDays(45)
                });
            }

            return offers;
        }
    }
}
