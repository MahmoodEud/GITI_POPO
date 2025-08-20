namespace ITI_GProject.DTOs
{
    public class AssessmentSummaryDto
    {
        public int AssessmentId { get; set; }
        public string AssessmentName { get; set; } = "";
        public int AttemptsCount { get; set; }
        public int UniqueStudents { get; set; }
        public double AverageScore { get; set; }
        public double MedianScore { get; set; }
        public double PassRate { get; set; } // 0..100
        public DateTime? LastAttemptAt { get; set; }
    }
}
