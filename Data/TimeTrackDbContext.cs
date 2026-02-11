using Microsoft.EntityFrameworkCore;
using TimeTrack.API.Models;
using TaskModel = TimeTrack.API.Models.Task;

namespace TimeTrack.API.Data
{
    public class TimeTrackDbContext : DbContext
    {
        public TimeTrackDbContext(DbContextOptions<TimeTrackDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TaskEntity> Tasks { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<TimeLogEntity> TimeLogs { get; set; }
        public DbSet<TaskTimeEntity> TaskTimes { get; set; }
        public DbSet<PendingRegistrationEntity> PendingRegistrations { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure TaskEntity -> UserEntity relationships
            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskEntity>()
                .HasOne(t => t.CreatedByUser)
                .WithMany(u => u.CreatedTasks)
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ProjectEntity -> UserEntity relationship
            modelBuilder.Entity<ProjectEntity>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(p => p.ManagerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}