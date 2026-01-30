using System.ComponentModel.DataAnnotations;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class LoanApplicationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public CarType CarType { get; set; }

        [Required]
        [Range(100000, 100000000)]
        public decimal CarPrice { get; set; }

        [Required]
        [Range(10000, 10000000)]
        public decimal MonthlyIncome { get; set; }

        public string? EmploymentType { get; set; }

        [Required]
        [Range(300, 900)]
        public int CreditScore { get; set; }

        [Required]
        [Range(10, 90)]
        public decimal DownPaymentPercent { get; set; }

        [Required]
        [Range(1, 15)]
        public int Tenure { get; set; }
    }
}
