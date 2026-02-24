using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.Analytics;

public class TeamMemberPerformanceDto
{
    [JsonPropertyName("members")]
    public List<MemberPerformance> Members { get; set; } = new();
}

public class MemberPerformance
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("totalHours")]
    public decimal TotalHours { get; set; }

    [JsonPropertyName("tasksAssigned")]
    public int TasksAssigned { get; set; }

    [JsonPropertyName("tasksCompleted")]
    public int TasksCompleted { get; set; }

    [JsonPropertyName("tasksInProgress")]
    public int TasksInProgress { get; set; }

    [JsonPropertyName("tasksPending")]
    public int TasksPending { get; set; }

    [JsonPropertyName("efficiencyScore")]
    public decimal EfficiencyScore { get; set; }

    [JsonPropertyName("performanceStatus")]
    public string PerformanceStatus { get; set; } = string.Empty;

    [JsonPropertyName("averageTaskCompletionTime")]
    public decimal AverageTaskCompletionTime { get; set; }

    [JsonPropertyName("overdueTasksCount")]
    public int OverdueTasksCount { get; set; }
}
