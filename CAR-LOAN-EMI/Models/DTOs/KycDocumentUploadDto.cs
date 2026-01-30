using Microsoft.AspNetCore.Http;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class KycDocumentUploadDto
    {
        public int UserId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public IFormFile File { get; set; } = null!;
    }
}
