using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<EmiPayment> CreateAsync(EmiPayment payment);
        Task<EmiPayment?> GetByIdAsync(int paymentId);
    }
}
