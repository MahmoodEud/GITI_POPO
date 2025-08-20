namespace ITI_GProject.DTOs
{
    public class StudentPerformanceDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;

        public int AttemptsCount { get; set; }
        public double AverageScore { get; set; }
        public double PassRate { get; set; }
    }
}
