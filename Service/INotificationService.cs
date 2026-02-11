using TimeTrack.API.Models;

namespace TimeTrack.API.Service;

public interface INotificationService
{
    System.Threading.Tasks.Task CreateNotificationAsync(int userId, string type, string message);
    System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetUserNotificationsAsync(int userId);
    System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetUnreadNotificationsAsync(int userId);
    System.Threading.Tasks.Task<int> GetUnreadCountAsync(int userId);
    System.Threading.Tasks.Task MarkAsReadAsync(int notificationId);
    System.Threading.Tasks.Task MarkAllAsReadAsync(int userId);
    System.Threading.Tasks.Task SendTaskAssignmentNotificationAsync(int userId, string taskTitle);
    System.Threading.Tasks.Task SendLogReminderNotificationAsync(int userId);
    System.Threading.Tasks.Task SendTaskDeadlineNotificationAsync(int userId, string taskTitle, DateTime dueDate);
}