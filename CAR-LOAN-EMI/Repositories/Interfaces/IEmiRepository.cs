using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface IEmiRepository
    {
        Task<List<EmiPayment>> GetByLoanIdAsync(int loanId);
        Task<List<EmiPayment>> GetUpcomingByUserIdAsync(int userId);
        Task<List<EmiPayment>> GetHistoryByUserIdAsync(int userId);
        Task<EmiPayment> CreateAsync(EmiPayment emiPayment);
        Task<EmiPayment> UpdateAsync(EmiPayment emiPayment);
    }
}
