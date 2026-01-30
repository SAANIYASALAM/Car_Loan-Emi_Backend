namespace CAR_LOAN_EMI.Models.DTOs
{
    public class UserFilterDto
    {
        public string? KycStatus { get; set; }
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
    }
}
