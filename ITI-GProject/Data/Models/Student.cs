using ITI_GProject.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace ITI_GProject.Data.Models
{
    public class Student
    {
        [Key]
        public int  Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string FName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LName { get; set; }
        [Required]

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

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters")]
        public string Passowrd { get; set; }

        [Required]
        public bool IsActive { get; set; }

        //many to many relation between course and student
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = new HashSet<StudentCourse>();

        //many to many relation between student and quiz 
        public virtual ICollection<StudentQuiz> StudentQuizzes { get; set; } = new HashSet<StudentQuiz>();


        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new HashSet<StudentAssignment>();


    }
}
