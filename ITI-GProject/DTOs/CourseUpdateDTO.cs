namespace ITI_GProject.DTOs
{
    public class CourseUpdateDTO
    {
        [Required]
        [StringLength(50)]
        public string Title { get; set; } = null!;
        [Required]
        [MaxLength(50)]

        public string Category { get; set; } = null!;
        [Required]
        public StudentYear Year { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [MaxLength(500)]

        public string Description { get; set; } = null!;
        [Required]
        public bool Status { get; set; }
        [FromForm]
        public IFormFile? PicturalUrl { get; set; }   
    }
}
