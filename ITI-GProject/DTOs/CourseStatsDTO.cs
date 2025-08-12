namespace ITI_GProject.DTOs
{
    public class CourseStatsDTO
    {
      
            public int Total { get; set; }
            public int Available { get; set; }
            public int Unavailable { get; set; }
            public int Free { get; set; }
            public int Paid { get; set; }
            public Dictionary<string, int> ByYear { get; set; } = new();
        
    }
}
