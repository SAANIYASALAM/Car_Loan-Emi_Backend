using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("KycDocuments")]
    public class KycDocument
    {
        [Key]
        public int KycDocumentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty; // Aadhar, PAN, Address Proof

        [Required]
        [StringLength(100)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [StringLength(500)]
        public string? FileUrl { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public KycStatus Status { get; set; } = KycStatus.Pending;

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
