namespace ITI_GProject.Services
{
    public interface IReportsService
    {
        Task<IEnumerable<AssessmentSummaryDto>> GetAssessmentsSummary();
        Task<IEnumerable<QuestionDifficultyDto>> GetQuestionDifficulty(int assessmentId);
        Task<IEnumerable<StudentPerformanceDto>> GetStudentsPerformance(int? assessmentId = null);
        Task<IEnumerable<AttemptsTimeSeriesDto>> GetAttemptsTimeSeries(int? assessmentId = null, int days = 30);
    }
}
