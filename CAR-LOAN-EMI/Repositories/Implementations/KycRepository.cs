using Microsoft.EntityFrameworkCore;
using CAR_LOAN_EMI.Data;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Repositories.Interfaces;

namespace CAR_LOAN_EMI.Repositories.Implementations
{
    public class KycRepository : IKycRepository
    {
        private readonly ApplicationDbContext _context;

        public KycRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<KycDocument?> GetByIdAsync(int documentId)
        {
            return await _context.KycDocuments
                .Include(k => k.User)
                .FirstOrDefaultAsync(k => k.KycDocumentId == documentId);
        }

        public async Task<List<KycDocument>> GetByUserIdAsync(int userId)
        {
            return await _context.KycDocuments
                .Where(k => k.UserId == userId)
                .OrderByDescending(k => k.UploadedAt)
                .ToListAsync();
        }

        public async Task<KycDocument> CreateAsync(KycDocument document)
        {
            _context.KycDocuments.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }

        public async Task<KycDocument> UpdateAsync(KycDocument document)
        {
            _context.KycDocuments.Update(document);
            await _context.SaveChangesAsync();
            return document;
        }
    }
}
