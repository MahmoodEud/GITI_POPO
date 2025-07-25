namespace ITI_GProject.DTOs
{
    public class LessonUpdateDto
    {
        [Required]
        [MaxLength(500)]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = "Video")]
        [DataType(DataType.Url)]
        public string Video_URL { get; set; } = null!;
        [DataType(DataType.Url)]
        public string? AbstructVideo { get; set; } = null!;
        [Required]
        public string PDF { get; set; } = null!;
    }
}
