namespace ITI_GProject.DTOs
{
    public class StudentAttemptDTO
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = default!;
        public int AssessmentId { get; set; }
        public int AttemptNumber { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? TimeLimitMinutes { get; set; }
        public int? Score { get; set; }
        public bool IsGraded { get; set; }
    }
}
