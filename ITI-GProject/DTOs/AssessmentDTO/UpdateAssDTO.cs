using ITI_GProject.DTOs.QuestionsDTO;

namespace ITI_GProject.DTOs.AssessmentDTO
{
    public class UpdateAssDTO
    {
        public int? Max_Attempts { get; set; }
        public int Passing_Score { get; set; }
        public int? Time_Limit { get; set; }
        public DateTime Starting_At { get; set; }
        public int? LessonId { get; set; }
        public List<QuesDTO> Questions { get; set; } = new List<QuesDTO>();
    }
}
