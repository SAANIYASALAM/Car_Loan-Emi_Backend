using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("LoanRules")]
    public class LoanRule
    {
        [Key]
        public int RuleId { get; set; }

        [Required]
        public CarType CarType { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal BaseRate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal MinDownPaymentPercent { get; set; }

        [StringLength(200)]
        public string? RiskFactor { get; set; }

        [StringLength(200)]
        public string? Discount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
