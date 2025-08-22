using Microsoft.AspNetCore.Authorization;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReportsStudentController : ControllerBase
{

    private readonly IStudentReportService _reportService;
    private readonly AppGConetxt _ctx;

    public ReportsStudentController(IStudentReportService reportService, AppGConetxt ctx)
    {
        _reportService = reportService;
        _ctx = ctx;
    }

    private async Task<int?> GetStudentIdAsync()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return null;

        var student = await _ctx.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId);

        return student?.Id;
    }

    [HttpGet("attempt/{attemptId:int}")]
    public async Task<ActionResult<StudentReportDto>> GetReport(int attemptId)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == null) return Unauthorized();

        var report = await _reportService.GetReportByAttemptIdAsync(attemptId, studentId.Value);
        if (report == null) return NotFound("Report not available for this attempt.");

        return Ok(report);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<StudentAttemptSummaryDto>>> GetMyAttempts()
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == null) return Unauthorized();

        var list = await _reportService.GetAllReportsAsync(studentId.Value);
        return Ok(list);
    }

    [HttpGet("overview")]
    public async Task<ActionResult<StudentReportsOverviewDto>> GetOverview([FromQuery] int recent = 10)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId == null) return Unauthorized();

        recent = recent < 1 ? 1 : (recent > 50 ? 50 : recent);

        var dto = await _reportService.GetOverviewAsync(studentId.Value, recent);
        return Ok(dto);
    }
}



