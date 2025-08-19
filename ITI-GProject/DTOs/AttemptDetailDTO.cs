namespace ITI_GProject.DTOs
{
    namespace ITI_GProject.DTOs.StudentAttemptsDTO
    {
        public class AttemptDetailDTO
        {
            public int AttemptId { get; set; }
            public int AssessmentId { get; set; }
            public DateTime StartedAt { get; set; }
            public DateTime? SubmittedAt { get; set; }
            public int Score { get; set; }
            public List<StudentAnswerDTO> Answers { get; set; } = new();
        }

        public class StudentAnswerDTO
        {
            public int QuestionId { get; set; }
            public string QuestionText { get; set; }
            public int SelectedChoiceId { get; set; }
            public bool IsCorrect { get; set; }
        }
    }


}
