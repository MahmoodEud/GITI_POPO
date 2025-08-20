using ITI_GProject.DTOs.QuestionsDTO;

namespace ITI_GProject.DTOs.AssessmentDTO
{
    public class CreateNewDTO
    {
        public int? MaxAttempts{ get; set; }
        public int PassingScore{ get; set; }
        public int? TimeLimit{ get; set; }
        public DateTime StartingAt{ get; set; }
        public int? LessonId { get; set; }
        public List<QuesDTO> Questions { get; set; } = new();



    }
}
