using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmiPayment> CreateAsync(EmiPayment payment)
        {
            _context.EmiPayments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<EmiPayment?> GetByIdAsync(int paymentId)
        {
            return await _context.EmiPayments
                .Include(p => p.Loan)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }
    }
}
