using CAR_LOAN_EMI.Models.DTOs;
using CAR_LOAN_EMI.Models.Entities;
using CAR_LOAN_EMI.Models.Enums;
using CAR_LOAN_EMI.Repositories.Interfaces;
using CAR_LOAN_EMI.Services.Interfaces;

namespace CAR_LOAN_EMI.Services.Implementations
{
    public class KycService : IKycService
    {
        private readonly IKycRepository _kycRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;

        public KycService(
            IKycRepository kycRepository, 
            IUserRepository userRepository,
            IWebHostEnvironment environment)
        {
            _kycRepository = kycRepository;
            _userRepository = userRepository;
            _environment = environment;
        }

        public async Task<ApiResponseDto<object>> UploadDocumentAsync(KycDocumentUploadDto dto)
        {
            try
            {
                // Validate user exists
                var user = await _userRepository.GetByIdAsync(dto.UserId);
                if (user == null)
                    return ApiResponseDto<object>.ErrorResponse("User not found");

                // Save file to disk
                var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", "kyc");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{dto.UserId}_{dto.DocumentType}_{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.File.CopyToAsync(stream);
                }

                // Create KYC document record
                var kycDocument = new KycDocument
                {
                    UserId = dto.UserId,
                    DocumentType = dto.DocumentType,
                    DocumentNumber = dto.DocumentNumber,
                    FilePath = filePath,
                    FileUrl = $"/uploads/kyc/{fileName}",
                    Status = KycStatus.Pending,
                    UploadedAt = DateTime.UtcNow
                };

                await _kycRepository.CreateAsync(kycDocument);

                return ApiResponseDto<object>.SuccessResponse(new
                {
                    kycDocument.KycDocumentId,
                    kycDocument.DocumentType,
                    kycDocument.FileUrl,
                    Status = kycDocument.Status.ToString()
                }, "Document uploaded successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to upload document: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<List<object>>> GetUserKycDocumentsAsync(int userId)
        {
            try
            {
                var documents = await _kycRepository.GetByUserIdAsync(userId);
                
                var documentData = documents.Select(d => new
                {
                    d.KycDocumentId,
                    d.DocumentType,
                    d.DocumentNumber,
                    d.FileUrl,
                    Status = d.Status.ToString(),
                    d.UploadedAt,
                    d.RejectionReason
                }).Cast<object>().ToList();

                return ApiResponseDto<List<object>>.SuccessResponse(documentData, "KYC documents retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<List<object>>.ErrorResponse($"Failed to retrieve KYC documents: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<object>> GetKycStatusAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return ApiResponseDto<object>.ErrorResponse("User not found");

                var documents = await _kycRepository.GetByUserIdAsync(userId);
                
                var totalDocuments = documents.Count;
                var verifiedDocuments = documents.Count(d => d.Status == KycStatus.Verified);
                var completionPercentage = totalDocuments > 0 ? (verifiedDocuments * 100 / totalDocuments) : 0;

                return ApiResponseDto<object>.SuccessResponse(new
                {
                    userId,
                    kycStatus = user.KycStatus,
                    totalDocuments,
                    verifiedDocuments,
                    completionPercentage,
                    documents = documents.Select(d => new
                    {
                        d.DocumentType,
                        Status = d.Status.ToString()
                    })
                }, "KYC status retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to retrieve KYC status: {ex.Message}");
            }
        }

        public async Task<ApiResponseDto<object>> VerifyDocumentAsync(int documentId)
        {
            try
            {
                var document = await _kycRepository.GetByIdAsync(documentId);
                if (document == null)
                    return ApiResponseDto<object>.ErrorResponse("Document not found");

                document.Status = KycStatus.Verified;
                await _kycRepository.UpdateAsync(document);

                // Update user KYC status if all documents are verified
                var userDocuments = await _kycRepository.GetByUserIdAsync(document.UserId);
                if (userDocuments.All(d => d.Status == KycStatus.Verified))
                {
                    var user = await _userRepository.GetByIdAsync(document.UserId);
                    if (user != null)
                    {
                        user.KycStatus = "Verified";
                        await _userRepository.UpdateAsync(user);
                    }
                }

                return ApiResponseDto<object>.SuccessResponse(new
                {
                    documentId,
                    Status = document.Status.ToString()
                }, "Document verified successfully");
            }
            catch (Exception ex)
            {
                return ApiResponseDto<object>.ErrorResponse($"Failed to verify document: {ex.Message}");
            }
        }
    }
}
