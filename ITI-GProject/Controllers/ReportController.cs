using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _service;
        public ReportsController(IReportsService service) { _service = service; }

        [HttpGet("assessments-summary")]
        public async Task<ActionResult<IEnumerable<AssessmentSummaryDto>>> GetAssessmentsSummary()
            => Ok(await _service.GetAssessmentsSummary());

        [HttpGet("assessment/{assessmentId}/questions-difficulty")]
        public async Task<ActionResult<IEnumerable<QuestionDifficultyDto>>> GetQuestionDifficulty(int assessmentId)
            => Ok(await _service.GetQuestionDifficulty(assessmentId));

        [HttpGet("students-performance")]
        public async Task<ActionResult<IEnumerable<StudentPerformanceDto>>> GetStudentsPerformance([FromQuery] int? assessmentId = null)
            => Ok(await _service.GetStudentsPerformance(assessmentId));

        [HttpGet("attempts-timeseries")]
        public async Task<ActionResult<IEnumerable<AttemptsTimeSeriesDto>>> GetAttemptsTimeSeries([FromQuery] int? assessmentId = null, [FromQuery] int days = 30)
            => Ok(await _service.GetAttemptsTimeSeries(assessmentId, days));
    }
}
