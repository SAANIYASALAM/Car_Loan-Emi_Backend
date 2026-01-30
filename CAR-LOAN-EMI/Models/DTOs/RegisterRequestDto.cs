using System.ComponentModel.DataAnnotations;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Range(10000, 10000000)]
        public decimal MonthlyIncome { get; set; }

        public string? EmploymentType { get; set; }

        [Range(300, 900)]
        public int CreditScore { get; set; } = 0;
    }
}
