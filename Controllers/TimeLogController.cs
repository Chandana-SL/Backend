using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeTrack.API.DTOs.Common;
using TimeTrack.API.DTOs.TimeLog;
using TimeTrack.API.Service;

namespace TimeTrack.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimeLogController : ControllerBase
{
    private readonly ITimeLoggingService _timeLoggingService;
    private readonly IBreakService _breakService;
    private readonly ILogger<TimeLogController> _logger;

    public TimeLogController(ITimeLoggingService timeLoggingService, IBreakService breakService, ILogger<TimeLogController> logger)
    {
        _timeLoggingService = timeLoggingService;
        _breakService = breakService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<TimeLogResponseDto>>> CreateTimeLog([FromBody] CreateTimeLogDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation(
            "[CreateTimeLog] UserId: {UserId}, Date: {Date}, StartTime: {StartTime}, EndTime: {EndTime}, BreakDuration: {BreakDuration}, TotalHours: {TotalHours}",
            userId, dto.Date, dto.StartTime, dto.EndTime, dto.BreakDuration, dto.TotalHours);

        try
        {
            var result = await _timeLoggingService.CreateTimeLogAsync(userId, dto);

            _logger.LogInformation(
                "[CreateTimeLog] Success - LogId: {LogId} generated for UserId: {UserId}",
                result.LogId, userId);

            return Ok(ApiResponseDto<TimeLogResponseDto>.SuccessResponse(result, "Time log created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, 
                "[CreateTimeLog] Failed - Duplicate log for UserId: {UserId}, Date: {Date}",
                userId, dto.Date);
            throw; // Re-throw to let GlobalExceptionHandler handle the response
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[CreateTimeLog] Database error for UserId: {UserId}. Exception: {ExceptionType}",
                userId, ex.GetType().Name);
            throw; // Re-throw to let GlobalExceptionHandler handle the response
        }
    }

    [HttpPut("{logId}")]
    public async Task<ActionResult<ApiResponseDto<TimeLogResponseDto>>> UpdateTimeLog(Guid logId, [FromBody] CreateTimeLogDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation(
            "[UpdateTimeLog] LogId: {LogId}, UserId: {UserId}, Date: {Date}, StartTime: {StartTime}, EndTime: {EndTime}, BreakDuration: {BreakDuration}, TotalHours: {TotalHours}",
            logId, userId, dto.Date, dto.StartTime, dto.EndTime, dto.BreakDuration, dto.TotalHours);

        try
        {
            var result = await _timeLoggingService.UpdateTimeLogAsync(logId, userId, dto);

            _logger.LogInformation(
                "[UpdateTimeLog] Success - LogId: {LogId} updated by UserId: {UserId}",
                logId, userId);

            return Ok(ApiResponseDto<TimeLogResponseDto>.SuccessResponse(result, "Time log updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, 
                "[UpdateTimeLog] Failed - LogId: {LogId} not found for UserId: {UserId}",
                logId, userId);
            throw; // Re-throw to let GlobalExceptionHandler handle the response
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, 
                "[UpdateTimeLog] Failed - UserId: {UserId} attempted to update LogId: {LogId} they don't own",
                userId, logId);
            throw; // Re-throw to let GlobalExceptionHandler handle the response
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[UpdateTimeLog] Database error for LogId: {LogId}, UserId: {UserId}. Exception: {ExceptionType}",
                logId, userId, ex.GetType().Name);
            throw; // Re-throw to let GlobalExceptionHandler handle the response
        }
    }

    [HttpDelete("{logId}")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteTimeLog(Guid logId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _timeLoggingService.DeleteTimeLogAsync(logId, userId);
        return Ok(ApiResponseDto<bool>.SuccessResponse(result, "Time log deleted successfully"));
    }

    [HttpGet("{logId}")]
    public async Task<ActionResult<ApiResponseDto<TimeLogResponseDto>>> GetTimeLogById(Guid logId)
    {
        var result = await _timeLoggingService.GetTimeLogByIdAsync(logId);
        return Ok(ApiResponseDto<TimeLogResponseDto>.SuccessResponse(result));
    }

    [HttpGet("user")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<TimeLogResponseDto>>>> GetUserTimeLogs(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _timeLoggingService.GetUserTimeLogsAsync(userId, startDate, endDate);
        return Ok(ApiResponseDto<IEnumerable<TimeLogResponseDto>>.SuccessResponse(result));
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpPost("{logId}/approve")]
    public async Task<ActionResult<ApiResponseDto<bool>>> ApproveTimeLog(Guid logId)
    {
        var managerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _timeLoggingService.ApproveTimeLogAsync(logId, managerId);
        return Ok(ApiResponseDto<bool>.SuccessResponse(result, "Time log approved successfully"));
    }

    [HttpGet("total-hours")]
    public async Task<ActionResult<ApiResponseDto<decimal>>> GetTotalHours(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _timeLoggingService.CalculateTotalHoursAsync(userId, startDate, endDate);
        return Ok(ApiResponseDto<decimal>.SuccessResponse(result));
    }

    [Authorize(Policy = "ManagerOrAdmin")]
    [HttpGet("team/{managerId}")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<DTOs.TimeLog.TeamTimeLogDto>>>> GetTeamTimeLogs(Guid managerId)
    {
        var logs = await _timeLoggingService.GetTeamTimeLogsByManagerIdAsync(managerId);

        if (logs == null || !logs.Any())
            return NotFound(ApiResponseDto<IEnumerable<DTOs.TimeLog.TeamTimeLogDto>>.ErrorResponse("No team members or time logs found for the given manager."));

        return Ok(ApiResponseDto<IEnumerable<DTOs.TimeLog.TeamTimeLogDto>>.SuccessResponse(logs));
    }

    [HttpGet("{timeLogId}/breaks")]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<DTOs.Break.BreakResponseDto>>>> GetBreaksForTimeLog(Guid timeLogId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        _logger.LogInformation("[GetBreaksForTimeLog] UserId: {UserId}, TimeLogId: {TimeLogId}", userId, timeLogId);

        try
        {
            var result = await _breakService.GetBreaksForTimeLogAsync(timeLogId, userId);
            return Ok(ApiResponseDto<IEnumerable<DTOs.Break.BreakResponseDto>>.SuccessResponse(result));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "[GetBreaksForTimeLog] TimeLogId: {TimeLogId} not found", timeLogId);
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "[GetBreaksForTimeLog] Unauthorized access to TimeLogId: {TimeLogId}", timeLogId);
            throw;
        }
    }
}