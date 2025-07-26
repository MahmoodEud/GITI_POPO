using ITI_GProject.DTOs.QuestionsDTO;

namespace ITI_GProject.DTOs.AssessmentDTO
{
    public class UpdateAssDTO
    {
        [Range(1, 100, ErrorMessage = "Max attempts must be 1-100")]
        public int? Max_Attempts { get; set; }

        [Range(0, 100, ErrorMessage = "Passing score must be 0-100")]
        public int? Passing_Score { get; set; }

        [Range(1, 100, ErrorMessage = "Duration must be 1-100 minutes")]
        public int? Time_Limit { get; set; }

        public DateTime? Starting_At { get; set; }

        public int? LessonId { get; set; }
        public List<QuesDTO> Questions { get; set; } = new List<QuesDTO>();
    }
}
