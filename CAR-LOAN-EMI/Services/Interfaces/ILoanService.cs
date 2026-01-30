using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface ILoanService
    {
        Task<ApiResponseDto<Loan>> ApplyForLoanAsync(LoanApplicationDto application);
        Task<ApiResponseDto<EligibilityCheckDto>> CheckEligibilityAsync(LoanApplicationDto application);
        Task<Loan?> GetLoanByIdAsync(int loanId);
        Task<List<Loan>> GetUserLoansAsync(int userId);
        LoanRuleDto GetLoanRulesAsync(CarType carType);
        decimal CalculateEmi(EmiCalculationDto calculation);
    }
}
