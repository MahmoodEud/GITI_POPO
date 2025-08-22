namespace ITI_GProject.DTOs
{
    public class MeetingNotificationRequest
    {
        public int CourseId { get; set; }
        public int? Year { get; set; }          
        public string Title { get; set; } = "";
        public string JoinUrl { get; set; } = "";
        public DateTime? StartAt { get; set; }  
        public DateTime? EndAt { get; set; }
    }
}
