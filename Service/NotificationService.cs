using TimeTrack.API.Models;
using TimeTrack.API.Repository.IRepository;
using TaskAsync = System.Threading.Tasks.Task;

namespace TimeTrack.API.Service;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async TaskAsync CreateNotificationAsync(int userId, string type, string message)
    {
        var notification = new NotificationEntity
        {
            UserId = userId,
            Type = type,
            Message = message,
            Status = "Unread",
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    public async System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetUserNotificationsAsync(int userId)
    {
        return await _unitOfWork.Notifications.GetNotificationsByUserIdAsync(userId);
    }

    public async System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetUnreadNotificationsAsync(int userId)
    {
        return await _unitOfWork.Notifications.GetUnreadNotificationsAsync(userId);
    }

    public async System.Threading.Tasks.Task<int> GetUnreadCountAsync(int userId)
    {
        return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
    }

    public async TaskAsync MarkAsReadAsync(int notificationId)
    {
        await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async TaskAsync MarkAllAsReadAsync(int userId)
    {
        await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async TaskAsync SendTaskAssignmentNotificationAsync(int userId, string taskTitle)
    {
        var message = $"New task assigned: '{taskTitle}'. Please review and start working on it.";
        await CreateNotificationAsync(userId, "TaskAssigned", message);
    }

    public async TaskAsync SendLogReminderNotificationAsync(int userId)
    {
        var message = "Reminder: Please log your work hours for today.";
        await CreateNotificationAsync(userId, "LogReminder", message);
    }

    public async TaskAsync SendTaskDeadlineNotificationAsync(int userId, string taskTitle, DateTime dueDate)
    {
        var daysRemaining = (dueDate.Date - DateTime.UtcNow.Date).Days;
        var urgency = daysRemaining <= 1 ? "urgent" : $"due in {daysRemaining} days";
        var message = $"Task '{taskTitle}' is {urgency}. Please complete it by {dueDate:MMM dd, yyyy}.";

        await CreateNotificationAsync(userId, "TaskDeadline", message);
    }
}