using CAR_LOAN_EMI.Helpers;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class PaymentCalendarService : IPaymentCalendarService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IEmiRepository _emiRepository;

        public PaymentCalendarService(ILoanRepository loanRepository, IEmiRepository emiRepository)
        {
            _loanRepository = loanRepository;
            _emiRepository = emiRepository;
        }

        public async Task<ApiResponseDto<List<PaymentCalendarDto>>> GeneratePaymentCalendarAsync(int loanId)
        {
            try
            {
                var loan = await _loanRepository.GetByIdAsync(loanId);
                if (loan == null)
                    return ApiResponseDto<List<PaymentCalendarDto>>.ErrorResponse("Loan not found");

                var emiPayments = await _emiRepository.GetByLoanIdAsync(loanId);

                var calendar = emiPayments.Select(emi => new PaymentCalendarDto
                {
                    EmiNumber = emi.EmiNumber,
                    DueDate = emi.DueDate,
                    Amount = emi.Amount,
                    Status = emi.Status.ToString(),
                    IsPaid = emi.Status == PaymentStatus.Paid,
                    IsOverdue = emi.Status == PaymentStatus.Overdue,
                    DaysUntilDue = (emi.DueDate - DateTime.UtcNow).Days,
                    Principal = CalculatePrincipalComponent(loan, emi.EmiNumber),
                    Interest = CalculateInterestComponent(loan, emi.EmiNumber)
                }).OrderBy(c => c.EmiNumber).ToList();

                return ApiResponseDto<List<PaymentCalendarDto>>.SuccessResponse(calendar, "Payment calendar generated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<PaymentCalendarDto>>.ErrorResponse($"Failed to generate payment calendar: {ex.Message}");
            }
        }

        private decimal CalculatePrincipalComponent(Models.Entities.Loan loan, int emiNumber)
        {
            // Calculate principal component using reducing balance method
            var monthlyRate = loan.InterestRate / 12 / 100;
            var totalEmis = loan.Tenure * 12;
            
            var remainingBalance = loan.LoanAmount;
            for (int i = 1; i < emiNumber; i++)
            {
                var interestForMonth = remainingBalance * monthlyRate;
                var principalForMonth = loan.EmiAmount - interestForMonth;
                remainingBalance -= principalForMonth;
            }

            var currentMonthInterest = remainingBalance * monthlyRate;
            return loan.EmiAmount - currentMonthInterest;
        }

        private decimal CalculateInterestComponent(Models.Entities.Loan loan, int emiNumber)
        {
            // Calculate interest component
            var monthlyRate = loan.InterestRate / 12 / 100;
            
            var remainingBalance = loan.LoanAmount;
            for (int i = 1; i < emiNumber; i++)
            {
                var interestForMonth = remainingBalance * monthlyRate;
                var principalForMonth = loan.EmiAmount - interestForMonth;
                remainingBalance -= principalForMonth;
            }

            return remainingBalance * monthlyRate;
        }
    }
}
