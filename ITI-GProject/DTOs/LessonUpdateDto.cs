namespace ITI_GProject.DTOs
{
    public class LessonUpdateDto
    {

        [Required, MaxLength(150)]
        public string title { get; set; } = null!;

        [DataType(DataType.Url)]
        [Required, Display(Name = "Video")]
        public string videoUrl { get; set; } = null!;
        public string? previewVideoUrl { get; set; }
        public string pdfUrl { get; set; } = null!;

        [Required] public int courseId { get; set; }
    }
}
