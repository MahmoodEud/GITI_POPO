namespace ITI_GProject.DTOs
{
    public class CourseProgressDTO
    {
        public int CourseId { get; set; }
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public double Percent { get; set; } 
        public List<LessonProgressDTO> Lessons { get; set; } = new();
    }
}
