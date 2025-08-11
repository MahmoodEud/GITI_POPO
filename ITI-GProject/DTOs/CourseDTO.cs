namespace ITI_GProject.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }


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
        public string PicturalUrl { get; set; } = "default.jpg";
        public bool Status { get; set; }
    }
}
