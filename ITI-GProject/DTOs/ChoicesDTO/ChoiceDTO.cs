namespace ITI_GProject.DTOs.ChoicesDTO
{
    public class ChoiceDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نص الاختيار مطلوب")]
        public string ChoiceText { get; set; }

        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

    }
}
