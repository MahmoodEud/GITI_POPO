namespace ITI_GProject.DTOs
{
    public class NotificationCreateRequest
    {
        public int StudentId { get; set; }
        public string Title { get; set; } = "";
        public string? Body { get; set; }

        public NotificationType Type { get; set; } = NotificationType.General;
        public string? ActionUrl { get; set; }
    }
}
