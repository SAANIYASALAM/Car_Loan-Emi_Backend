using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class EmiService : IEmiService
    {
        private readonly IEmiRepository _emiRepository;
        private readonly ILoanRepository _loanRepository;

        public EmiService(IEmiRepository emiRepository, ILoanRepository loanRepository)
        {
            _emiRepository = emiRepository;
            _loanRepository = loanRepository;
        }

        public async Task<List<EmiPayment>> GetEmiPaymentsByLoanIdAsync(int loanId)
        {
            return await _emiRepository.GetByLoanIdAsync(loanId);
        }

        public async Task<List<EmiPayment>> GetUpcomingPaymentsAsync(int userId)
        {
            return await _emiRepository.GetUpcomingByUserIdAsync(userId);
        }

        public async Task<List<EmiPayment>> GetPaymentHistoryAsync(int userId)
        {
            return await _emiRepository.GetHistoryByUserIdAsync(userId);
        }

        public async Task<ApiResponseDto<EmiPayment>> ProcessPaymentAsync(EmiPaymentDto paymentDto)
        {
            try
            {
                // Get the loan
                var loan = await _loanRepository.GetByIdAsync(paymentDto.LoanId);
                if (loan == null)
                {
                    return ApiResponseDto<EmiPayment>.ErrorResponse("Loan not found");
                }

                // Get pending EMI payments for this loan
                var pendingPayments = await _emiRepository.GetByLoanIdAsync(paymentDto.LoanId);
                var nextPendingPayment = pendingPayments
                    .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue)
                    .OrderBy(p => p.EmiNumber)
                    .FirstOrDefault();

                if (nextPendingPayment == null)
                {
                    return ApiResponseDto<EmiPayment>.ErrorResponse("No pending payments found for this loan");
                }

                // Update payment status
                nextPendingPayment.Status = PaymentStatus.Paid;
                nextPendingPayment.PaymentDate = DateTime.UtcNow;
                nextPendingPayment.PaymentMethod = paymentDto.PaymentMethod;
                nextPendingPayment.TransactionId = paymentDto.TransactionId;

                var updatedPayment = await _emiRepository.UpdateAsync(nextPendingPayment);

                // Update loan details
                loan.PaidAmount += paymentDto.Amount;
                loan.RemainingEmis--;

                // Update next due date
                if (loan.RemainingEmis > 0)
                {
                    var nextPayment = pendingPayments
                        .Where(p => p.Status == PaymentStatus.Pending && p.EmiNumber > nextPendingPayment.EmiNumber)
                        .OrderBy(p => p.EmiNumber)
                        .FirstOrDefault();
                    
                    if (nextPayment != null)
                    {
                        loan.NextDueDate = nextPayment.DueDate;
                    }
                }
                else
                {
                    loan.NextDueDate = null;
                    loan.Status = LoanStatus.Closed;
                }

                await _loanRepository.UpdateAsync(loan);

                return ApiResponseDto<EmiPayment>.SuccessResponse(updatedPayment, "Payment processed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<EmiPayment>.ErrorResponse("Payment processing failed: " + ex.Message);
            }
        }
    }
}
