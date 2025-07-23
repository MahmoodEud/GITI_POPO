using Microsoft.AspNetCore.Mvc;
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
        [DataType(DataType.Url)]
        public string Video_URL { get; set; }
        [DataType(DataType.Url)]
        public string? AbstructVideo { get; set; }
        [Required]
        public string PDF { get; set; }

        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        public virtual ICollection<Assessments>? Quizzes { get; set; } = new HashSet<Assessments>();



    }
}
