using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_GProject.Data.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 100, ErrorMessage = "Max attempts must be 1-100")]
        [Display(Name = "Maximum Attempts")]
        public int Max_Attempts { get; set; }
        [Required]

        public int Passing_Score { get; set; }
        [Range(1, 100, ErrorMessage = "Duration must be 1-100 minutes")]
        [Display(Name = "Duration (minutes)")]
        public int Time_Limit { get; set; }
        [Required]
        public DateTime Starting_At { get; set; }

        [ForeignKey("Lesson")]

        public int? LessonId { get; set; }
        public Lesson? Lesson { get; set; }


        //many to many relation between student and quiz 
        public virtual ICollection<StudentQuiz> StudentQuizzes { get; set; } = new HashSet<StudentQuiz>();



    }
}
