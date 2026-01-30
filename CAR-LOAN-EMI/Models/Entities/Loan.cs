using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("Loans")]
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CarPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LoanAmount { get; set; }

        [Required]
        public CarType CarType { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        public int Tenure { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmiAmount { get; set; }

        [Required]
        public LoanStatus Status { get; set; } = LoanStatus.Pending;

        public int RemainingEmis { get; set; }

        public DateTime? NextDueDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal DownPaymentPercent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DownPaymentAmount { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        public DateTime? ApprovalDate { get; set; }

        public DateTime? DisbursementDate { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        
        public virtual ICollection<EmiPayment> EmiPayments { get; set; } = new List<EmiPayment>();
    }
}
