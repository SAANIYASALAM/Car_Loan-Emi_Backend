using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class EmiRepository : IEmiRepository
    {
        private readonly ApplicationDbContext _context;

        public EmiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<EmiPayment>> GetByLoanIdAsync(int loanId)
        {
            return await _context.EmiPayments
                .Where(e => e.LoanId == loanId)
                .OrderBy(e => e.EmiNumber)
                .ToListAsync();
        }

        public async Task<List<EmiPayment>> GetUpcomingByUserIdAsync(int userId)
        {
            return await _context.EmiPayments
                .Include(e => e.Loan)
                .Where(e => e.Loan.UserId == userId && 
                           (e.Status == PaymentStatus.Pending || e.Status == PaymentStatus.Overdue))
                .OrderBy(e => e.DueDate)
                .ToListAsync();
        }

        public async Task<List<EmiPayment>> GetHistoryByUserIdAsync(int userId)
        {
            return await _context.EmiPayments
                .Include(e => e.Loan)
                .Where(e => e.Loan.UserId == userId && e.Status == PaymentStatus.Paid)
                .OrderByDescending(e => e.PaymentDate)
                .ToListAsync();
        }

        public async Task<EmiPayment> CreateAsync(EmiPayment emiPayment)
        {
            _context.EmiPayments.Add(emiPayment);
            await _context.SaveChangesAsync();
            return emiPayment;
        }

        public async Task<EmiPayment> UpdateAsync(EmiPayment emiPayment)
        {
            _context.EmiPayments.Update(emiPayment);
            await _context.SaveChangesAsync();
            return emiPayment;
        }
    }
}
