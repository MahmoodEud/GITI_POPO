namespace ITI_GProject.DTOs
{
    public class CourseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Category { get; set; } = null!;
        public StudentYear Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = null!;
        public string PicturalUrl { get; set; } = "default.jpg";
        public string? Level { get; set; }
        public bool IsAvailable { get; set; }
        public CourseStatus Status { get; set; }
    }
}
