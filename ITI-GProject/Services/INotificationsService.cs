namespace ITI_GProject.Services
{
    public interface INotificationsService
    {
        Task<NotificationDto> CreateAsync(int studentId, string title, string body,
            NotificationType type = NotificationType.General, string? actionUrl = null);

        Task<int> CreateManyAsync(IEnumerable<int> studentIds, string title, string body,
            NotificationType type = NotificationType.General, string? actionUrl = null);

        Task<List<int>> ResolveAudienceAsync(int? year, int? courseId);

        Task<IReadOnlyList<NotificationDto>> GetByStudentAsync(int studentId, int? take = null);
        Task<int> GetUnreadCountAsync(int studentId);
        Task<bool> MarkAsReadAsync(int notificationId, int studentId);
        Task<int> MarkAllAsReadAsync(int studentId);
    }
}
