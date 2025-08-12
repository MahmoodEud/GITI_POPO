namespace ITI_GProject.Data.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }=null!;
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }=null!;
        [Required]
        public StudentYear Year { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } =null!;
        [Required]
        public bool IsAvailable { get; set; }
        [Column(TypeName =("decimal(8,2)"))]
        public decimal Price { get; set; }
        public string PicturalUrl { get; set; } = default!;

        [MaxLength(50)]
        public string? Level { get; set; }
        [Required]
        public CourseStatus Status { get; set; } = CourseStatus.Draft;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        //one to many relation between course and lesson
        public virtual ICollection<Lesson>? Lessons { get; set; } = new HashSet<Lesson>();
        /// Many to many relation between student and course 
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();


    }
}
