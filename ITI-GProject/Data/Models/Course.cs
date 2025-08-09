namespace ITI_GProject.Data.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Title { get; set; }
        [Required]
        [MaxLength(50)]

        public string Category { get; set; }
        [Required]
        public StudentYear Year { get; set; }
        [Required]
        [MaxLength(500)]
        public string Description { get; set; }
        [Required]
        public bool Status { get; set; }
        [Column(TypeName =("decimal(8,2)"))]
        public decimal Price { get; set; }
        public string PicturalUrl { get; set; } = default!;
        //one to many relation between course and lesson
        public virtual ICollection<Lesson>? Lessons { get; set; } = new HashSet<Lesson>();
        /// Many to many relation between student and course 
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();


    }
}
