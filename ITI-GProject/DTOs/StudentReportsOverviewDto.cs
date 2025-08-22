namespace ITI_GProject.DTOs
{
    public class StudentReportsOverviewDto
    {
        public int TotalAttempts { get; set; }
        public int DistinctAssessments { get; set; }
        public double AverageScore { get; set; }   
        public double PassRate { get; set; }       
        public int TotalCorrect { get; set; }
        public int TotalWrong { get; set; }
        public int ImprovementPoints { get; set; } 
        public List<RecentScorePointDto> RecentScores { get; set; } = new();
    }
}
