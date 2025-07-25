namespace ITI_GProject.Data.Models
{
    public class Student
    {
        [Key]
        public int  Id { get; set; }

        public StudentYear Year { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(011|012|015|010)\d{8}$",
        ErrorMessage = "Phone number must be 11 digits starting with 011, 012, 015, or 010")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]

        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Parent Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(011|012|015|010)\d{8}$",
         ErrorMessage = "Phone number must be 11 digits starting with 011, 012, 015, or 010")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
        public string ParentNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        //many to many relation between course and student
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();
        public virtual ICollection<StudentAttempts> StudentAttempts { get; set; } = new HashSet<StudentAttempts>();


        //many to many relation between student and quiz 
        public virtual ICollection<StudentAttempts> StudentQuizzes { get; set; } = new HashSet<StudentAttempts>();

        //ApplicationUser relation
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
