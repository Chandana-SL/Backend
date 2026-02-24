using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.Analytics;

public class TaskCompletionBreakdownDto
{
    [JsonPropertyName("completedCount")]
    public int CompletedCount { get; set; }

    [JsonPropertyName("inProgressCount")]
    public int InProgressCount { get; set; }

    [JsonPropertyName("pendingCount")]
    public int PendingCount { get; set; }

    [JsonPropertyName("rejectedCount")]
    public int RejectedCount { get; set; }

    [JsonPropertyName("overdueCount")]
    public int OverdueCount { get; set; }

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("completionPercentage")]
    public decimal CompletionPercentage { get; set; }
}
