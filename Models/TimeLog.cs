using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrack.API.Models
{
    public class TimeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LogId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public TimeSpan BreakDuration { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public bool IsApproved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // Constructor to ensure GUID is generated
        public TimeLog()
        {
            LogId = Guid.NewGuid();
        }
    }
}
