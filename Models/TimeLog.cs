using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrack.API.Models
{
    [Table("TimeLogs")]
    public class TimeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LogId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan? EndTime { get; set; }

        [Required]
        public int BreakDuration { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalHours { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Activity { get; set; }

        // Navigation Property
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        public ICollection<Break> Breaks { get; set; } = new List<Break>();

        // Constructor to ensure GUID is generated
        public TimeLog()
        {
            LogId = Guid.NewGuid();
        }
    }
}
