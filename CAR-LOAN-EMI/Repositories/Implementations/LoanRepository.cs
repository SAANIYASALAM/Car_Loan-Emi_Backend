using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Loan?> GetByIdAsync(int loanId)
        {
            return await _context.Loans
                .Include(l => l.User)
                .Include(l => l.EmiPayments)
                .FirstOrDefaultAsync(l => l.LoanId == loanId);
        }

        public async Task<List<Loan>> GetLoansByUserIdAsync(int userId)
        {
            return await _context.Loans
                .Include(l => l.EmiPayments)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<Loan> CreateAsync(Loan loan)
        {
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return loan;
        }

        public async Task<Loan> UpdateAsync(Loan loan)
        {
            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();
            return loan;
        }
    }
}
