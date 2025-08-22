namespace ITI_GProject.DTOs
{
    public class StudentReportDto
    {
        public int AttemptId { get; set; }
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; } = "";
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public double Percentage { get; set; }
        public bool IsPassed { get; set; }
        public List<QuestionReportDto> Questions { get; set; } = new();
    }
}
