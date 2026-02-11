using TimeTrack.API.Models;
using TaskAsync = System.Threading.Tasks.Task;

namespace TimeTrack.API.Data;

public static class DatabaseSeeder
{
    public static async TaskAsync SeedAsync(TimeTrackDbContext context)
    {
        // Check if admin user already exists
        if (!context.Users.Any(u => u.Email == "admin@backend.com"))
        {
            var admin = new UserEntity
            {
                Name = "System Administrator",
                Email = "admin@backend.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                Department = "IT",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };

            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}