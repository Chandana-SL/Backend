using Microsoft.EntityFrameworkCore;
using TimeTrack.API.Data;
using TimeTrack.API.Models;
using TimeTrack.API.Repository.IRepository;

namespace TimeTrack.API.Repository;

public class TimeLogRepository : GenericRepository<TimeLog>, ITimeLogRepository
{
    public TimeLogRepository(TimeTrackDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimeLog>> GetLogsByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeLog>> GetLogsByDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(t => t.User)
            .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeLog>> GetLogsByDepartmentAsync(string department, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(t => t.User)
            .Where(t => t.User.Department == department && t.Date >= startDate && t.Date <= endDate)
            .OrderBy(t => t.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalHoursByUserAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && t.Date >= startDate && t.Date <= endDate)
            .SumAsync(t => t.TotalHours);
    }

    public async Task<IEnumerable<TimeLog>> GetPendingApprovalLogsAsync(Guid managerId)
    {
        var managerDepartment = await _context.Users
            .Where(u => u.UserId == managerId)
            .Select(u => u.Department)
            .FirstOrDefaultAsync();

        return await _dbSet
            .Include(t => t.User)
            .Where(t => t.User.Department == managerDepartment)
            .OrderBy(t => t.Date)
            .ToListAsync();
    }

    public async Task<TimeLog?> GetLogByUserAndDateAsync(Guid userId, DateTime date)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.UserId == userId && t.Date.Date == date.Date);
    }

    public async Task<decimal> GetTotalHoursByUsersForDateAsync(IEnumerable<Guid> userIds, DateTime date)
    {
        if (userIds == null || !userIds.Any()) return 0m;
        return await _dbSet
            .Where(t => userIds.Contains(t.UserId) && t.Date.Date == date.Date)
            .SumAsync(t => t.TotalHours);
    }

    // Organization Analytics Methods
    public async Task<IEnumerable<TimeLog>> GetAllTimeLogsWithDetailsAsync(
        DateTime? startDate, 
        DateTime? endDate, 
        string? department, 
        string? status)
    {
        var query = _dbSet
            .Include(t => t.User)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        if (!string.IsNullOrEmpty(department))
            query = query.Where(t => t.User.Department == department);

        // Note: TimeLog doesn't have a Status field, so we'll skip this filter for now
        // If needed, you can add Status to TimeLog model

        return await query
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalHoursForOrganizationAsync(DateTime startDate, DateTime endDate)
    {
        var total = await _dbSet
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .SumAsync(t => (decimal?)t.TotalHours);

        return total ?? 0m;
    }

    public async Task<Dictionary<DateTime, decimal>> GetDailyHoursAggregateAsync(DateTime startDate, DateTime endDate)
    {
        var result = await _dbSet
            .Where(t => t.Date >= startDate && t.Date <= endDate)
            .GroupBy(t => t.Date.Date)
            .Select(g => new { Date = g.Key, TotalHours = g.Sum(t => t.TotalHours) })
            .ToDictionaryAsync(x => x.Date, x => x.TotalHours);

        return result;
    }
}