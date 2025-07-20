using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_GProject.Data.Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Title { get;set ; }
        [Required]
        public string Video_URL { get; set; }

        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public virtual ICollection<Quiz>? Quizzes { get; set; } = new HashSet<Quiz>();
        public virtual ICollection<Assignment>? Assignments { get; set; } = new HashSet<Assignment>();



    }
}
