using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class ContactMessageDto
    {
        public int? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ContactFilterDto
    {
        public ContactStatus? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
