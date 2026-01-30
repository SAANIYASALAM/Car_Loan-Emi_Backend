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

                // Get interest rate based on car type and credit score
                var interestRate = GetBaseInterestRate(application.CarType, application.CreditScore);
                if (application.EmploymentType == "Self-Employed")
                    interestRate += 0.5m;

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
                    Status = LoanStatus.Pending,  // Changed to Pending for admin approval
                    DownPaymentPercent = application.DownPaymentPercent,
                    DownPaymentAmount = downPaymentAmount,
                    RemainingEmis = application.Tenure * 12,
                    NextDueDate = DateTime.UtcNow.AddMonths(1),
                    ApplicationDate = DateTime.UtcNow,
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
                // 1. KYC Verification Check
                var user = await _userRepository.GetByIdAsync(application.UserId);
                if (user == null)
                    return CreateRejectionResponse("User not found");

                if (user.KycStatus != "Verified")
                    return CreateRejectionResponse("KYC not verified. Please complete KYC verification.");

                // 2. Age Validation
                if (application.UserAge < 18)
                    return CreateRejectionResponse("User must be at least 18 years old.");

                // 3. Credit Score Minimum
                if (application.CreditScore < 650)
                    return CreateRejectionResponse("Credit score below 650 is not eligible.");

                // 4. Loan Amount Validation
                var loanAmount = application.CarPrice - (application.CarPrice * application.DownPaymentPercent / 100);
                if (loanAmount <= 0)
                    return CreateRejectionResponse("Loan amount must be greater than zero.");

                // 5. Down Payment Check (Min 10%)
                if (application.DownPaymentPercent < 10)
                    return CreateRejectionResponse("Down payment must be at least 10% of car price.");

                // 6. Tenure Validation
                if (application.Tenure < 1 || application.Tenure > 7)
                    return CreateRejectionResponse("Tenure must be between 1 and 7 years.");

                // 7. Employment-Based Income Validation
                if (application.EmploymentType == "Salaried" && application.MonthlyIncome < 25000)
                    return CreateRejectionResponse("Minimum monthly income for Salaried employees is ₹25,000.");

                if (application.EmploymentType == "Self-Employed" && application.MonthlyIncome < 40000)
                    return CreateRejectionResponse("Minimum monthly income for Self-Employed individuals is ₹40,000.");

                // 8. Interest Rate Adjustment
                var interestRate = GetBaseInterestRate(application.CarType, application.CreditScore);
                if (application.EmploymentType == "Self-Employed")
                    interestRate += 0.5m;

                // 9. Car Type-Specific Loan-to-Value Ratio
                var loanToValueRatio = (loanAmount / application.CarPrice) * 100;
                var carTypeValidation = ValidateCarTypeRules(application.CarType, loanToValueRatio, interestRate, application.DownPaymentPercent);
                if (!carTypeValidation.IsValid)
                    return CreateRejectionResponse(carTypeValidation.Message);

                // 10. EMI Calculation
                var emi = EmiCalculator.CalculateEMI(loanAmount, interestRate, application.Tenure);

                // 11. Affordability Check (EMI <= 40% of Income)
                var maxAllowedEMI = application.MonthlyIncome * 0.40m;
                if (emi > maxAllowedEMI)
                    return CreateRejectionResponse($"EMI (₹{emi:N0}) exceeds 40% of monthly income limit (₹{maxAllowedEMI:N0}).");

                // All checks passed
                return ApiResponseDto<EligibilityCheckDto>.SuccessResponse(new EligibilityCheckDto
                {
                    Eligible = true,
                    EstimatedEmi = emi,
                    InterestRate = interestRate,
                    Reason = "Eligible for loan"
                }, "Eligibility check successful");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<EligibilityCheckDto>.ErrorResponse("Eligibility check failed: " + ex.Message);
            }
        }

        private ApiResponseDto<EligibilityCheckDto> CreateRejectionResponse(string reason)
        {
            return ApiResponseDto<EligibilityCheckDto>.SuccessResponse(new EligibilityCheckDto
            {
                Eligible = false,
                Reason = reason,
                EstimatedEmi = 0,
                InterestRate = 0
            }, "Eligibility check completed");
        }

        private decimal GetBaseInterestRate(CarType carType, int creditScore)
        {
            // Base rate by credit score
            var baseRate = creditScore switch
            {
                >= 800 => 8.5m,
                >= 750 => 9.0m,
                >= 700 => 9.5m,
                >= 650 => 10.0m,
                _ => 12.0m
            };

            // Adjust by car type
            var carTypeAdjustment = carType switch
            {
                CarType.ElectricVehicle => -0.5m,  // Green loan discount
                CarType.UsedCar => 2.0m,            // Higher rate for used cars
                CarType.LuxurySedan or CarType.LuxurySUV => 1.0m,
                _ => 0m
            };

            return baseRate + carTypeAdjustment;
        }

        private CarTypeValidationResult ValidateCarTypeRules(CarType carType, decimal ltvRatio, decimal interestRate, decimal downPaymentPercent)
        {
            return carType switch
            {
                CarType.Hatchback => ltvRatio > 90
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for Hatchback is 90% of car price." }
                    : (interestRate < 8.5m || interestRate > 10.5m)
                    ? new CarTypeValidationResult { IsValid = false, Message = "Hatchback interest rate must be between 8.5% and 10.5%." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                CarType.Sedan => ltvRatio > 85
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for Sedan is 85% of car price." }
                    : (interestRate < 9m || interestRate > 11.5m)
                    ? new CarTypeValidationResult { IsValid = false, Message = "Sedan interest rate must be between 9% and 11.5%." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                CarType.MidSizeSUV or CarType.FullSizeSUV => ltvRatio > 80
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for SUV is 80% of car price." }
                    : (interestRate < 9.5m || interestRate > 12m)
                    ? new CarTypeValidationResult { IsValid = false, Message = "SUV interest rate must be between 9.5% and 12%." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                CarType.LuxurySedan or CarType.LuxurySUV => ltvRatio > 70
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for Luxury cars is 70% of car price." }
                    : downPaymentPercent < 30
                    ? new CarTypeValidationResult { IsValid = false, Message = "Luxury cars require minimum 30% down payment." }
                    : (interestRate < 10.5m || interestRate > 13m)
                    ? new CarTypeValidationResult { IsValid = false, Message = "Luxury car interest rate must be between 10.5% and 13%." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                CarType.ElectricVehicle => ltvRatio > 90
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for EV is 90% of car price." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                CarType.UsedCar => ltvRatio > 70
                    ? new CarTypeValidationResult { IsValid = false, Message = "Max loan for Used cars is 70% of car price." }
                    : (interestRate < 11m || interestRate > 14m)
                    ? new CarTypeValidationResult { IsValid = false, Message = "Used car interest rate must be between 11% and 14%." }
                    : new CarTypeValidationResult { IsValid = true, Message = "" },

                _ => new CarTypeValidationResult { IsValid = true, Message = "" }
            };
        }

        private class CarTypeValidationResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; } = string.Empty;
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
