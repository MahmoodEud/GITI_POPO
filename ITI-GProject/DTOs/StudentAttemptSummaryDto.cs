namespace ITI_GProject.DTOs
{
    public class StudentAttemptSummaryDto
    {
        public int AttemptId { get; set; }
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; } = string.Empty;
        public int Score { get; set; }                
        public int TotalQuestions { get; set; }        
        public double Percentage { get; set; }         
        public bool IsPassed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
