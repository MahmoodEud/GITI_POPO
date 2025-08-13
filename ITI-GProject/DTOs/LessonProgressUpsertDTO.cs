namespace ITI_GProject.DTOs
{
    public class LessonProgressUpsertDTO
    {
        public int WatchedSeconds { get; set; }
        public int? DurationSeconds { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
