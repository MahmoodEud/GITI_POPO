namespace ITI_GProject.DTOs
{
    public class QuestionReportDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = "";
        public string StudentAnswer { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
        public bool IsCorrect { get; set; }
    }
}
