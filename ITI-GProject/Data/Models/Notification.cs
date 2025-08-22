namespace ITI_GProject.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Body { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;

        // جديد
        public NotificationType Type { get; set; } = NotificationType.General;

        [MaxLength(1024)]
        public string? ActionUrl { get; set; }
    }
}

