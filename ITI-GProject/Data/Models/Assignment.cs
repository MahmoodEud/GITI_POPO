using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_GProject.Data.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public int Max_Points { get; set; }
        [Required]
        public DateTime Due_Date { get; set; }

        [ForeignKey("Lesson")]
        public int? LessonId { get; set; }   
        public Lesson? Lesson{get;set;}

        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new HashSet<StudentAssignment>();


    }
}
