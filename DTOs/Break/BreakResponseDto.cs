using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.Break;

public class BreakResponseDto
{
    [JsonPropertyName("breakId")]
    public Guid BreakId { get; set; }

    [JsonPropertyName("timeLogId")]
    public Guid TimeLogId { get; set; }

    [JsonPropertyName("activity")]
    public string Activity { get; set; } = string.Empty;

    [JsonPropertyName("startTime")]
    public string StartTime { get; set; } = string.Empty;

    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("modifiedAt")]
    public DateTime ModifiedAt { get; set; }
}
