using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        public string KycStatus { get; set; } = "Pending";

        public int CreditScore { get; set; } = 0;

        [StringLength(500)]
        public string? ProfileImage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; } = 0;

        [StringLength(50)]
        public string? EmploymentType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public virtual ICollection<KycDocument> KycDocuments { get; set; } = new List<KycDocument>();
    }
}
