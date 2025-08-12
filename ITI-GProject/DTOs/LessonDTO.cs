namespace ITI_GProject.DTOs
{
    public class LessonDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Title { get; set; } = null!;

        [DataType(DataType.Url)]
        [Required, Display(Name = "Video")]
        public string VideoUrl { get; set; } = null!;
        public string? PreviewVideoUrl { get; set; }
        public string PdfUrl { get; set; } = null!;

        [Required] public int courseId { get; set; }

    }
    }
