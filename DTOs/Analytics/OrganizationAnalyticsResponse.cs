namespace TimeTrack.API.DTOs.Analytics;

/// <summary>
/// Combined response containing all analytics data for organization-wide metrics
/// </summary>
public class OrganizationAnalyticsResponse
{
    // Summary Metrics
    public decimal TotalHoursLogged { get; set; }
    public decimal AvgHoursPerEmployee { get; set; }
    public int ActiveEmployees { get; set; }
    public int TotalEmployees { get; set; }
    
    // Task Metrics
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int PendingTasks { get; set; }
    public decimal TaskCompletionPercentage { get; set; }
    
    // Role Distribution
    public int EmployeeCount { get; set; }
    public int ManagerCount { get; set; }
    public int AdminCount { get; set; }
    
    // Department Data
    public List<DepartmentAnalyticsDto> DepartmentMetrics { get; set; } = new();
    public decimal AvgEmployeesPerDepartment { get; set; }
    
    // Time Series Data
    public List<DailyHoursDto> HoursTrendData { get; set; } = new();
    
    // Response metadata
    public DateTime ReportGeneratedAt { get; set; }
    public string PeriodRange { get; set; } = string.Empty;
}
