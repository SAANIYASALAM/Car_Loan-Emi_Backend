using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
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

        public async Task<List<Loan>> GetPendingLoansAsync()
        {
            return await _context.Loans
                .Include(l => l.User)
                .Where(l => l.Status == LoanStatus.Pending)
                .OrderByDescending(l => l.ApplicationDate)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetAllLoansAsync(LoanFilterDto? filter = null)
        {
            var query = _context.Loans.Include(l => l.User).AsQueryable();

            if (filter != null)
            {
                if (filter.Status.HasValue)
                    query = query.Where(l => l.Status == filter.Status.Value);

                if (filter.CarType.HasValue)
                    query = query.Where(l => l.CarType == filter.CarType.Value);

                if (filter.FromDate.HasValue)
                    query = query.Where(l => l.ApplicationDate >= filter.FromDate.Value);

                if (filter.ToDate.HasValue)
                    query = query.Where(l => l.ApplicationDate <= filter.ToDate.Value);
            }

            return await query.OrderByDescending(l => l.ApplicationDate).ToListAsync();
        }

        public async Task<List<Loan>> GetApprovedLoansSince(DateTime startDate)
        {
            return await _context.Loans
                .Where(l => l.Status != LoanStatus.Rejected && l.ApplicationDate >= startDate)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetAllActiveLoans()
        {
            return await _context.Loans
                .Where(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Approved)
                .ToListAsync();
        }
    }
}
