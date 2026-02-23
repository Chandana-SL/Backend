using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.User;

public class DashboardStatsDto
{
    [JsonPropertyName("totalTeamMembers")]
    public int TotalTeamMembers { get; set; }

    [JsonPropertyName("activeTasks")]
    public int ActiveTasks { get; set; }

    [JsonPropertyName("totalTeamHoursToday")]
    public decimal TotalTeamHoursToday { get; set; }
}
