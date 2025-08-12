namespace ITI_GProject.Data.Models
{
    public class Lesson
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [DataType(DataType.Url)]
        public string VideoUrl { get; set; } = null!;
        [DataType(DataType.Url)]
        public string? PreviewVideoUrl { get; set; }
        public string PdfUrl { get; set; } = null!;

        [Required] public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        public int Order { get; set; } = 0;
        public TimeSpan? Duration { get; set; }
        public bool IsPreview { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Assessments>? Quizzes { get; set; } = new HashSet<Assessments>();



    }
}
