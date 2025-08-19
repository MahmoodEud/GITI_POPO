using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_GProject.Data.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [MinLength(5)]
        [MaxLength(100)]
        public string Header { get; set; } = string.Empty;

        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        [MinLength(10)]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "الإجابة يجب أن تكون نصًا واضحًا بدون رموز.")]
        public string correctAnswer { get; set; } = string.Empty;

        // FK لجدول Assessments (إسم الخاصية كما هو مستخدم عندكم في الخدمات/الـ DTOs)
        [ForeignKey(nameof(assessment))]
        public int QuizId { get; set; }

        // Navigation
        public Assessments assessment { get; set; } = null!;

        public virtual ICollection<Choice>? choices { get; set; } = new HashSet<Choice>();
        public virtual ICollection<StudentResponse>? StudentResponses { get; set; } = new HashSet<StudentResponse>();
    }
}
