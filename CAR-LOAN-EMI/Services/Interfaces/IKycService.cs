using CAR_LOAN_EMI.Models.DTOs;

namespace CAR_LOAN_EMI.Services.Interfaces
{
    public interface IKycService
    {
        Task<ApiResponseDto<object>> UploadDocumentAsync(KycDocumentUploadDto dto);
        Task<ApiResponseDto<List<object>>> GetUserKycDocumentsAsync(int userId);
        Task<ApiResponseDto<object>> GetKycStatusAsync(int userId);
        Task<ApiResponseDto<object>> VerifyDocumentAsync(int documentId);
    }
}
