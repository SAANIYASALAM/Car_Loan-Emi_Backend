using System.ComponentModel.DataAnnotations;

namespace CAR_LOAN_EMI.Models.DTOs
{
    public class EmiPaymentDto
    {
        [Required]
        public int LoanId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }
    }
}
