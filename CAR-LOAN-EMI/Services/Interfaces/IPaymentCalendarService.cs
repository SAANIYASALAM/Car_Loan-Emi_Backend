using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IPaymentCalendarService
    {
        Task<ApiResponseDto<List<PaymentCalendarDto>>> GeneratePaymentCalendarAsync(int loanId);
    }
}
