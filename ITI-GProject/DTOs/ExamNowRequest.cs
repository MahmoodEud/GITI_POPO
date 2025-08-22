namespace ITI_GProject.DTOs
{
    public class ExamNowRequest
    {
        public int Year { get; set; }
        public int CourseId { get; set; }
        public int LessonId { get; set; }   
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
