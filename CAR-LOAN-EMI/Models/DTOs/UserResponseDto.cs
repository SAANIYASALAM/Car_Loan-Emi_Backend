namespace CAR_LOAN_EMI.Models.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string KycStatus { get; set; } = string.Empty;
        public int CreditScore { get; set; }
        public string? ProfileImage { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string? EmploymentType { get; set; }
    }
}
