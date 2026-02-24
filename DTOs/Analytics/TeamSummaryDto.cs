using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.Analytics;

public class TeamSummaryDto
{
    [JsonPropertyName("totalTeamHours")]
    public decimal TotalTeamHours { get; set; }

    [JsonPropertyName("averageHoursPerMember")]
    public decimal AverageHoursPerMember { get; set; }

    [JsonPropertyName("completionRate")]
    public int CompletionRate { get; set; }

    [JsonPropertyName("completedTasksCount")]
    public int CompletedTasksCount { get; set; }

    [JsonPropertyName("totalTasksCount")]
    public int TotalTasksCount { get; set; }

    [JsonPropertyName("teamMemberCount")]
    public int TeamMemberCount { get; set; }

    [JsonPropertyName("calculatedFrom")]
    public DateTime CalculatedFrom { get; set; }

    [JsonPropertyName("calculatedTo")]
    public DateTime CalculatedTo { get; set; }
}
