using CAR_LOAN_EMI.Models.Entities;

namespace CAR_LOAN_EMI.Repositories.Interfaces
{
    public interface IKycRepository
    {
        Task<KycDocument?> GetByIdAsync(int documentId);
        Task<List<KycDocument>> GetByUserIdAsync(int userId);
        Task<KycDocument> CreateAsync(KycDocument document);
        Task<KycDocument> UpdateAsync(KycDocument document);
    }
}
