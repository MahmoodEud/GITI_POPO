namespace ITI_GProject.DTOs
{
    public class LessonProgressDTO
    {
        public int LessonId { get; set; }
        public int WatchedSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
