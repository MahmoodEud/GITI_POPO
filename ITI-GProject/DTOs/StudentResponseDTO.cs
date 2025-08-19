namespace ITI_GProject.DTOs
{
    public class StudentResponseDTO
    {
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
