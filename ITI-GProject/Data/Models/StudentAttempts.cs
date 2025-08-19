namespace ITI_GProject.Data.Models
{
    public class StudentAttempts
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "StudentId مطلوب")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "AssessmentId مطلوب")]
        public int AssessmentId { get; set; }

        [Range(1, 100, ErrorMessage = "عدد المحاولات يجب أن يكون بين 1 و 100")]
        public int AttemptsNumber { get; set; }   

        [Required(ErrorMessage = "وقت البدء مطلوب")]
        public DateTime StartedAt { get; set; }

        public DateTime? SubmittedAt { get; set; }  

        public int? Score { get; set; }   

        // Navigation properties
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [ForeignKey(nameof(AssessmentId))]
        public virtual Assessments Assessment { get; set; } = null!;

        public virtual ICollection<StudentResponse> StudentResponses { get; set; } = new HashSet<StudentResponse>();
    }

}
