namespace ITI_GProject.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }
        public string? ActionUrl { get; set; }
    }
}
