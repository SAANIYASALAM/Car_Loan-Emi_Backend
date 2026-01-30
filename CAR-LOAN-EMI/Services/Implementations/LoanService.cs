using CAR_LOAN_EMI.Helpers;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmiRepository _emiRepository;
        private readonly LoanRulesEngine _loanRulesEngine;

        public LoanService(
            ILoanRepository loanRepository,
            IUserRepository userRepository,
            IEmiRepository emiRepository,
            LoanRulesEngine loanRulesEngine)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
            _emiRepository = emiRepository;
            _loanRulesEngine = loanRulesEngine;
        }

        public async Task<ApiResponseDto<Loan>> ApplyForLoanAsync(LoanApplicationDto application)
        {
            try
            {
                // Check eligibility first
                var eligibilityCheck = await CheckEligibilityAsync(application);
                if (!eligibilityCheck.Data!.Eligible)
                {
                    return ApiResponseDto<Loan>.ErrorResponse(eligibilityCheck.Data.Reason);
                }

                // Get loan rules
                var loanRules = _loanRulesEngine.GetLoanRules(application.CarType);

                // Calculate down payment
                var downPaymentAmount = application.CarPrice * (application.DownPaymentPercent / 100);
                var loanAmount = application.CarPrice - downPaymentAmount;

                // Get interest rate based on credit score
                var interestRate = GetInterestRateByScore(application.CreditScore);

                // Calculate EMI
                var emiAmount = EmiCalculator.CalculateEMI(loanAmount, interestRate, application.Tenure);

                // Create loan
                var loan = new Loan
                {
                    UserId = application.UserId,
                    CarPrice = application.CarPrice,
                    LoanAmount = loanAmount,
                    CarType = application.CarType,
                    InterestRate = interestRate,
                    Tenure = application.Tenure,
                    EmiAmount = emiAmount,
                    Status = LoanStatus.Approved,
                    DownPaymentPercent = application.DownPaymentPercent,
                    DownPaymentAmount = downPaymentAmount,
                    RemainingEmis = application.Tenure * 12,
                    NextDueDate = DateTime.UtcNow.AddMonths(1),
                    ApplicationDate = DateTime.UtcNow,
                    ApprovalDate = DateTime.UtcNow,
                    DisbursementDate = DateTime.UtcNow,
                    PaidAmount = 0
                };

                var createdLoan = await _loanRepository.CreateAsync(loan);

                // Create EMI payment schedule
                for (int i = 1; i <= loan.RemainingEmis; i++)
                {
                    var emiPayment = new EmiPayment
                    {
                        LoanId = createdLoan.LoanId,
                        Amount = emiAmount,
                        DueDate = DateTime.UtcNow.AddMonths(i),
                        Status = PaymentStatus.Pending,
                        EmiNumber = i,
                        PaymentDate = DateTime.MinValue,
                        LateFee = 0,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _emiRepository.CreateAsync(emiPayment);
                }

                return ApiResponseDto<Loan>.SuccessResponse(createdLoan, "Loan application approved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<Loan>.ErrorResponse("Loan application failed: " + ex.Message);
            }
        }

        public async Task<ApiResponseDto<EligibilityCheckDto>> CheckEligibilityAsync(LoanApplicationDto application)
        {
            try
            {
                var eligibilityResult = new EligibilityCheckDto();

                // Check minimum credit score
                if (application.CreditScore < 650)
                {
                    eligibilityResult.Eligible = false;
                    eligibilityResult.Reason = "Credit score must be at least 650";
                    return ApiResponseDto<EligibilityCheckDto>.SuccessResponse(eligibilityResult);
                }

                // Calculate loan amount and EMI
                var downPaymentAmount = application.CarPrice * (application.DownPaymentPercent / 100);
                var loanAmount = application.CarPrice - downPaymentAmount;
                var interestRate = GetInterestRateByScore(application.CreditScore);
                var emiAmount = EmiCalculator.CalculateEMI(loanAmount, interestRate, application.Tenure);

                // Check EMI vs income ratio (max 40%)
                var emiToIncomeRatio = (emiAmount / application.MonthlyIncome) * 100;
                if (emiToIncomeRatio > 40)
                {
                    eligibilityResult.Eligible = false;
                    eligibilityResult.Reason = $"EMI (₹{emiAmount:N2}) exceeds 40% of monthly income (₹{application.MonthlyIncome:N2})";
                    eligibilityResult.EstimatedEmi = emiAmount;
                    eligibilityResult.InterestRate = interestRate;
                    return ApiResponseDto<EligibilityCheckDto>.SuccessResponse(eligibilityResult);
                }

                eligibilityResult.Eligible = true;
                eligibilityResult.Reason = "You are eligible for this loan";
                eligibilityResult.EstimatedEmi = emiAmount;
                eligibilityResult.InterestRate = interestRate;

                return ApiResponseDto<EligibilityCheckDto>.SuccessResponse(eligibilityResult);
            }
            catch (Exception ex)
            {
                return ApiResponseDto<EligibilityCheckDto>.ErrorResponse("Eligibility check failed: " + ex.Message);
            }
        }

        public async Task<Loan?> GetLoanByIdAsync(int loanId)
        {
            return await _loanRepository.GetByIdAsync(loanId);
        }

        public async Task<List<Loan>> GetUserLoansAsync(int userId)
        {
            return await _loanRepository.GetLoansByUserIdAsync(userId);
        }

        public LoanRuleDto GetLoanRulesAsync(CarType carType)
        {
            return _loanRulesEngine.GetLoanRules(carType);
        }

        public decimal CalculateEmi(EmiCalculationDto calculation)
        {
            return EmiCalculator.CalculateEMI(calculation.Principal, calculation.Rate, calculation.Tenure);
        }

        private decimal GetInterestRateByScore(int creditScore)
        {
            if (creditScore >= 800)
                return 8.5m;
            else if (creditScore >= 750)
                return 9.5m;
            else if (creditScore >= 650)
                return 11.5m;
            else
                return 15m;
        }
    }
}
