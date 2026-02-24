namespace TimeTrack.API.DTOs.Analytics;

/// <summary>
/// Detailed analytics for a single department
/// </summary>
public class DepartmentAnalyticsDto
{
    public string DepartmentName { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public decimal TotalHours { get; set; }
    public decimal AvgHoursPerEmployee { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int PendingTasks { get; set; }
    public List<string> EmployeeIds { get; set; } = new();
}
