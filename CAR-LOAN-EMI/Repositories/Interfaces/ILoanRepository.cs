using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(int loanId);
        Task<List<Loan>> GetLoansByUserIdAsync(int userId);
        Task<Loan> CreateAsync(Loan loan);
        Task<Loan> UpdateAsync(Loan loan);
    }
}
