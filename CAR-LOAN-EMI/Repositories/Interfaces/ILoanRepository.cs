using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<Loan?> GetByIdAsync(int loanId);
        Task<List<Loan>> GetLoansByUserIdAsync(int userId);
        Task<Loan> CreateAsync(Loan loan);
        Task<Loan> UpdateAsync(Loan loan);
        
        // New methods for admin functionality
        Task<List<Loan>> GetPendingLoansAsync();
        Task<List<Loan>> GetAllLoansAsync(LoanFilterDto? filter = null);
        Task<List<Loan>> GetApprovedLoansSince(DateTime startDate);
        Task<List<Loan>> GetAllActiveLoans();
    }
}
