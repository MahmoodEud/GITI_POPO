namespace ITI_GProject.Data.Models
{
    public class StudentLessonProgress
    {
        public int Id { get; set; }
        public int StudentId { get; set; }     
        public int LessonId { get; set; }     

        public int WatchedSeconds { get; set; }    
        public int DurationSeconds { get; set; }   
        public bool IsCompleted { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Student Student { get; set; } = null!;
        public Lesson Lesson { get; set; } = null!;
    }
}
