namespace ITI_GProject.DTOs
{
    public class CourseContentDTO
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public List<LessonDTO> Lessons { get; set; } = new();
    }
}
