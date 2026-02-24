using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrack.API.DTOs.Analytics;
using TimeTrack.API.DTOs.Common;
using TimeTrack.API.Service;

namespace TimeTrack.API.Controllers;

[Authorize(Policy = "ManagerOrAdmin")]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Gets team summary analytics for dashboard cards
    /// </summary>
    /// <param name="startDate">Optional start date (default: 30 days ago)</param>
    /// <param name="endDate">Optional end date (default: today)</param>
    [HttpGet("team-summary")]
    public async Task<ActionResult<ApiResponseDto<TeamSummaryDto>>> GetTeamSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _analyticsService.GetTeamSummaryAsync(managerId, startDate, endDate);
        return Ok(ApiResponseDto<TeamSummaryDto>.SuccessResponse(result, "Team summary retrieved successfully"));
    }

    /// <summary>
    /// Gets team hours trend data for line chart
    /// </summary>
    /// <param name="startDate">Required start date</param>
    /// <param name="endDate">Required end date</param>
    /// <param name="groupBy">Optional grouping: "day" or "week" (default: "day")</param>
    [HttpGet("team-hours-trend")]
    public async Task<ActionResult<ApiResponseDto<TeamHoursTrendDto>>> GetTeamHoursTrend(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string groupBy = "day")
    {
        // Validate date range
        if (startDate > endDate)
        {
            return BadRequest(ApiResponseDto<TeamHoursTrendDto>.ErrorResponse("Start date cannot be after end date"));
        }

        // Validate groupBy parameter
        if (!new[] { "day", "week" }.Contains(groupBy.ToLower()))
        {
            return BadRequest(ApiResponseDto<TeamHoursTrendDto>.ErrorResponse("Invalid groupBy parameter. Use 'day' or 'week'"));
        }

        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _analyticsService.GetTeamHoursTrendAsync(managerId, startDate, endDate, groupBy);
        return Ok(ApiResponseDto<TeamHoursTrendDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets individual performance metrics for each team member
    /// </summary>
    /// <param name="startDate">Optional start date (default: 30 days ago)</param>
    /// <param name="endDate">Optional end date (default: today)</param>
    [HttpGet("team-member-performance")]
    public async Task<ActionResult<ApiResponseDto<TeamMemberPerformanceDto>>> GetTeamMemberPerformance(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _analyticsService.GetTeamMemberPerformanceAsync(managerId, startDate, endDate);
        return Ok(ApiResponseDto<TeamMemberPerformanceDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets task completion breakdown by status for doughnut chart
    /// </summary>
    /// <param name="startDate">Optional start date (default: 30 days ago)</param>
    /// <param name="endDate">Optional end date (default: today)</param>
    [HttpGet("task-completion-breakdown")]
    public async Task<ActionResult<ApiResponseDto<TaskCompletionBreakdownDto>>> GetTaskCompletionBreakdown(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _analyticsService.GetTaskCompletionBreakdownAsync(managerId, startDate, endDate);
        return Ok(ApiResponseDto<TaskCompletionBreakdownDto>.SuccessResponse(result));
    }

    // ==================== ORGANIZATION ANALYTICS ENDPOINTS ====================

    /// <summary>
    /// Gets organization-wide analytics summary (Admin only)
    /// </summary>
    /// <param name="startDate">Optional start date for analytics</param>
    /// <param name="endDate">Optional end date for analytics</param>
    /// <param name="period">Optional period in days (7, 14, 30, 90) - overrides startDate if provided</param>
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("organization-summary")]
    public async Task<ActionResult<ApiResponseDto<OrganizationAnalyticsResponse>>> GetOrganizationSummary(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? period)
    {
        // Validate period if provided
        if (period.HasValue && !new[] { 7, 14, 30, 90 }.Contains(period.Value))
        {
            return BadRequest(ApiResponseDto<OrganizationAnalyticsResponse>.ErrorResponse(
                "Invalid period. Use 7, 14, 30, or 90 days"));
        }

        var result = await _analyticsService.GetOrganizationSummaryAsync(startDate, endDate, period);
        return Ok(ApiResponseDto<OrganizationAnalyticsResponse>.SuccessResponse(
            result, 
            "Organization analytics retrieved successfully"));
    }

    /// <summary>
    /// Gets detailed analytics for a specific department (Admin or Manager)
    /// </summary>
    /// <param name="departmentName">Name of the department</param>
    /// <param name="startDate">Optional start date</param>
    /// <param name="endDate">Optional end date</param>
    [HttpGet("department/{departmentName}")]
    public async Task<ActionResult<ApiResponseDto<DepartmentAnalyticsDto>>> GetDepartmentAnalytics(
        string departmentName,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        if (string.IsNullOrWhiteSpace(departmentName))
        {
            return BadRequest(ApiResponseDto<DepartmentAnalyticsDto>.ErrorResponse(
                "Department name is required"));
        }

        var result = await _analyticsService.GetDepartmentAnalyticsAsync(departmentName, startDate, endDate);
        return Ok(ApiResponseDto<DepartmentAnalyticsDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Gets daily hours trend data for organization chart (Admin only)
    /// </summary>
    /// <param name="days">Number of days to include (7, 14, 30, 90) - default: 7</param>
    [Authorize(Policy = "AdminOnly")]
    [HttpGet("hours-trend")]
    public async Task<ActionResult<ApiResponseDto<List<DailyHoursDto>>>> GetHoursTrend(
        [FromQuery] int days = 7)
    {
        // Validate days parameter
        if (!new[] { 7, 14, 30, 90 }.Contains(days))
        {
            return BadRequest(ApiResponseDto<List<DailyHoursDto>>.ErrorResponse(
                "Invalid days parameter. Use 7, 14, 30, or 90"));
        }

        var result = await _analyticsService.GetHoursTrendAsync(days);
        return Ok(ApiResponseDto<List<DailyHoursDto>>.SuccessResponse(result));
    }
}
