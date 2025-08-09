using ITI_GProject.DTOs.ChoicesDTO;

namespace ITI_GProject.DTOs.QuestionsDTO
{
    public class QuesDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Header { get; set; } = string.Empty;
        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        public string correctAnswer { get; set; } = string.Empty;

        [ForeignKey("quiz")]
        public int QuizId { get; set; }

        // Navigation Property
        //One Quistion Has Many Choices
        public List<ChoiceDTO> Choices { get; set; } = new List<ChoiceDTO>();

    }
}
