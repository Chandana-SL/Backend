namespace TimeTrack.API.DTOs.User;

/// <summary>
/// User information with role and department details for organization analytics
/// </summary>
public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? PunchedInAt { get; set; }
}
