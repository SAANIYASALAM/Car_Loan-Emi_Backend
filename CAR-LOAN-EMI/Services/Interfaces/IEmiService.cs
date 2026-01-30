using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IEmiService
    {
        Task<List<EmiPayment>> GetEmiPaymentsByLoanIdAsync(int loanId);
        Task<List<EmiPayment>> GetUpcomingPaymentsAsync(int userId);
        Task<List<EmiPayment>> GetPaymentHistoryAsync(int userId);
        Task<ApiResponseDto<EmiPayment>> ProcessPaymentAsync(EmiPaymentDto paymentDto);
    }
}
