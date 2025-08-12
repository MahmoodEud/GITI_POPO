namespace ITI_GProject.DTOs
{
    public class LessonUpdateDto
    {
        [Required]
        [MaxLength(500)]
        public string title { get; set; } = null!;
        [Required]
        [Display(Name = "Video")]
        [DataType(DataType.Url)]
        public string videoUrl { get; set; } = null!;
        [DataType(DataType.Url)]
        public string? previewVideoUrl { get; set; } = null!;
        [Required]
        public string pdfUrl { get; set; } = null!;
        [Required] public int courseId { get; set; }
    }
}
