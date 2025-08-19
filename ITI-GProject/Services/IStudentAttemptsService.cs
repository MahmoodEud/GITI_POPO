namespace ITI_GProject.Services
{
    public interface IStudentAttemptsService
    {
        Task<StudentAttemptDTO?> StartAttemptAsync(int studentId, int assessmentId);
        Task<StudentAttemptDTO?> SubmitAttemptAsync(int attemptId, List<StudentResponseDTO> responses);
        Task<IEnumerable<StudentAttemptDTO>> GetAttemptsByStudentAsync(int studentId);
        Task<IEnumerable<StudentAttemptDTO>> GetAttemptsByAssessmentAsync(int assessmentId);
    }
}
