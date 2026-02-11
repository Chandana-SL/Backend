using TimeTrack.API.Models;
using System.Threading.Tasks;

namespace TimeTrack.API.Repository.IRepository;

public interface INotificationRepository : IGenericRepository<NotificationEntity>
{
    System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetNotificationsByUserIdAsync(int userId);
    System.Threading.Tasks.Task<IEnumerable<NotificationEntity>> GetUnreadNotificationsAsync(int userId);
    System.Threading.Tasks.Task<int> GetUnreadCountAsync(int userId);
    System.Threading.Tasks.Task MarkAsReadAsync(int notificationId);
    System.Threading.Tasks.Task MarkAllAsReadAsync(int userId);
}