namespace ITI_GProject.Services
{
    public interface IStudentReportService
    {
        Task<StudentReportDto?> GetReportByAttemptIdAsync(int attemptId, int studentId);
        Task<IEnumerable<StudentAttemptSummaryDto>> GetAllReportsAsync(int studentId);
        Task<StudentReportsOverviewDto> GetOverviewAsync(int studentId, int recentCount = 10);

    }
}
