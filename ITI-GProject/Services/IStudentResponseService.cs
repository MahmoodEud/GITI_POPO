
namespace ITI_GProject.Services
{
    public interface IStudentResponseService
    {
        Task<StudentResponseDTO?> AddResponseAsync(int attemptId, StudentResponseCreateDTO responseDto);
        Task<IEnumerable<StudentResponseDTO>> GetResponsesByAttemptAsync(int attemptId);
        Task<StudentResponseDTO?> GetResponseAsync(int id);
        Task<bool> DeleteResponseAsync(int id);
    }

}
