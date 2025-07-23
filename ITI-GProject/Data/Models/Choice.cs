using System.ComponentModel.DataAnnotations.Schema;

namespace ITI_GProject.Data.Models
{
    public class Choice
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "نص الاختيار مطلوب")]
        public string ChoiceText { get; set; }

        public bool IsCorrect { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }

        public virtual Question question { get; set; }
    }
}
