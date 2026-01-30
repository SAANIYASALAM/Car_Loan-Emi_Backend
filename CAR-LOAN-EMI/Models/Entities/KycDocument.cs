using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("KycDocuments")]
    public class KycDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string DocumentPath { get; set; } = string.Empty;

        [StringLength(50)]
        public string VerificationStatus { get; set; } = "Pending";

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public DateTime? VerifiedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
