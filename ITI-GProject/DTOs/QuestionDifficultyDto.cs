namespace ITI_GProject.DTOs
{
    public class QuestionDifficultyDto
    {
        public int QuestionId { get; set; }
        public string Header { get; set; } = "";
        public int TotalAnswers { get; set; }
        public int WrongCount { get; set; }
        public double WrongRate { get; set; } // 0..100
    }
}
