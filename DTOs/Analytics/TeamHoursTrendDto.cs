using System.Text.Json.Serialization;

namespace TimeTrack.API.DTOs.Analytics;

public class TeamHoursTrendDto
{
    [JsonPropertyName("trendData")]
    public List<TrendDataPoint> TrendData { get; set; } = new();
}

public class TrendDataPoint
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("totalHours")]
    public decimal TotalHours { get; set; }

    [JsonPropertyName("tasksCompleted")]
    public int TasksCompleted { get; set; }

    [JsonPropertyName("activeMembers")]
    public int ActiveMembers { get; set; }
}
