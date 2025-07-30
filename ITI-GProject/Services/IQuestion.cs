using ITI_GProject.DTOs.QuestionsDTO;

namespace ITI_GProject.Services
{
    public interface IQuestion
    {
        Task<IEnumerable<QuesDTO>>GetQuestionByQuizId(int quizId);
        Task<QuesDTO> GetQuestionById(int id);
        Task<QuesDTO> CreateNewQuestion(QuesDTO dto);
        Task<QuesDTO> UpdateQuestion(int id, QuesDTO dto);
        Task<bool> DeletQuestion(int id);



    }
}
