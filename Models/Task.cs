using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrack.API.Models
{
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid TaskId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public Guid AssignedToUserId { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed

        [MaxLength(50)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High

        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation property
        [ForeignKey(nameof(AssignedToUserId))]
        public User AssignedToUser { get; set; } = null!;

        public ICollection<TaskTime> TaskTimes { get; set; } = new List<TaskTime>();

        // Constructor to ensure GUID is generated
        public Task()
        {
            TaskId = Guid.NewGuid();
        }
    }
}