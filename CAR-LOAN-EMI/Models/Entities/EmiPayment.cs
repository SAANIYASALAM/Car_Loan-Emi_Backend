using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("EmiPayments")]
    public class EmiPayment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int LoanId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime DueDate { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        public int EmiNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LateFee { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("LoanId")]
        public virtual Loan Loan { get; set; } = null!;
    }
}
