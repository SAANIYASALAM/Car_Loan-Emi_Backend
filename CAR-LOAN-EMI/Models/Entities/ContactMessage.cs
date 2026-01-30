using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CAR_LOAN_EMI.Models.Enums;

namespace CAR_LOAN_EMI.Models.Entities
{
    [Table("ContactMessages")]
    public class ContactMessage
    {
        [Key]
        public int MessageId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ContactStatus Status { get; set; } = ContactStatus.New;

        [StringLength(2000)]
        public string? Response { get; set; }

        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
