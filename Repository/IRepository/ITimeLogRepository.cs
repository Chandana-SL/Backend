using TimeTrack.API.Models;

namespace TimeTrack.API.Repository.IRepository;

public interface ITimeLogRepository : IGenericRepository<TimeLog>
{
    Task<IEnumerable<TimeLog>> GetLogsByUserIdAsync(Guid userId);
    Task<IEnumerable<TimeLog>> GetLogsByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeLog>> GetLogsByDepartmentAsync(string department, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalHoursByUserAsync(Guid userId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<TimeLog>> GetPendingApprovalLogsAsync(Guid managerId);
    Task<TimeLog?> GetLogByUserAndDateAsync(Guid userId, DateTime date);
    Task<decimal> GetTotalHoursByUsersForDateAsync(IEnumerable<Guid> userIds, DateTime date);

    // Organization Analytics Methods
    Task<IEnumerable<TimeLog>> GetAllTimeLogsWithDetailsAsync(DateTime? startDate, DateTime? endDate, string? department, string? status);
    Task<decimal> GetTotalHoursForOrganizationAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<DateTime, decimal>> GetDailyHoursAggregateAsync(DateTime startDate, DateTime endDate);
}