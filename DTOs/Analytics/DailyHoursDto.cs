namespace TimeTrack.API.DTOs.Analytics;

/// <summary>
/// Daily aggregated hours data for trend charts
/// </summary>
public class DailyHoursDto
{
    public DateTime Date { get; set; }
    public decimal TotalHours { get; set; }
    public int ActiveEmployees { get; set; }
    public string DateLabel { get; set; } = string.Empty;
}
