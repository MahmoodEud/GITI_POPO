
namespace ITI_GProject.Data.Models
{
    public class Assessments
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
        public virtual ICollection<StudentAttempts> StudentAttempts { get; set; } = new HashSet<StudentAttempts>();
        //one to many relation Between Quiz and Question
        public virtual ICollection<Question>? Questions { get; set; } = new HashSet<Question>();



    }
}
