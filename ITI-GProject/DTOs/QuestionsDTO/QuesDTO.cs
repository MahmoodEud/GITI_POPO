using ITI_GProject.DTOs.ChoicesDTO;

namespace ITI_GProject.DTOs.QuestionsDTO
{
    public class QuesDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        [MinLength(5, ErrorMessage = "العنوان يجب أن يحتوي على 5 أحرف على الأقل.")]
        [MaxLength(100, ErrorMessage = "العنوان لا يجب أن يزيد عن 100 حرف.")]
        public string Header { get; set; } = string.Empty;
        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        [MinLength(10, ErrorMessage = "النص يجب أن يحتوي على 10 أحرف على الأقل.")]
        public string Body { get; set; } = string.Empty;

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "الإجابة يجب أن تكون نصًا واضحًا بدون رموز.")]
        public string correctAnswer { get; set; } = string.Empty;
        [ForeignKey("quiz")]
        public int QuizId { get; set; }

        // Navigation Property
        //One Quistion Has Many Choices
        public List<ChoiceDTO> Choices { get; set; } = new List<ChoiceDTO>();

    }
}
