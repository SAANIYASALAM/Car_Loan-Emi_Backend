namespace CAR_LOAN_EMI.Models.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserResponseDto User { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
