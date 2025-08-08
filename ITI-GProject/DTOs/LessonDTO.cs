namespace ITI_GProject.DTOs
{
    public class LessonDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Title { get; set; }
        [Required]
        [Display(Name ="Video")]
        [DataType(DataType.Url)]
        public string? Video_URL { get; set; }
        [DataType(DataType.Url)]
        public string? AbstructVideo { get; set; }
        public string? PDF { get; set; }
    }
}
