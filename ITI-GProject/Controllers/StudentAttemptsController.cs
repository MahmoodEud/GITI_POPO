using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Controllers
{
    [ApiController]
    [Route("api/StudentAttempts")]
    [Authorize(Roles = "Student,Admin,Assistant")]
    public class StudentAttemptsController : ControllerBase
    {
        private readonly IStudentAttemptsService _service;
        private readonly AppGConetxt _ctx;

        public StudentAttemptsController(IStudentAttemptsService service, AppGConetxt ctx)
        {
            _service = service;
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

        [HttpPost("{assessmentId:int}/start")]
        public async Task<IActionResult> StartAttempt(int assessmentId)
        {
            var studentId = await GetStudentIdAsync();
            if (studentId is null)
                return Unauthorized(new { message = "Student profile not found." });

            var assessment = await _ctx.Assessments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == assessmentId);

            if (assessment == null)
                return NotFound(new { message = "Assessment not found." });

            var now = DateTime.Now;

            if (assessment.Starting_At > now)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = $"The assessment will start at {assessment.Starting_At:yyyy-MM-dd HH:mm:ss} UTC."
                });
            }

            if (assessment.Time_Limit.HasValue)
            {
                var endTime = assessment.Starting_At.AddMinutes(assessment.Time_Limit.Value);
                if (now > endTime)
                {
                    return StatusCode(StatusCodes.Status410Gone, new
                    {
                        message = "The assessment has expired."
                    });
                }
            }

            var attempt = await _service.StartAttemptAsync(studentId.Value, assessmentId);
            if (attempt is null)
            {
                return Conflict(new
                {
                    message = "Cannot start attempt. Maximum attempts reached or assessment not found."
                });
            }

            return Ok(attempt);
        }


        [HttpPost("{attemptId:int}/submit")]
        public async Task<IActionResult> SubmitAttempt(int attemptId, [FromBody] List<StudentResponseDTO> responses)
        {
            var attempt = await _ctx.StudentAttempts
                .Include(a => a.Assessment)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null) return NotFound(new { message = "Attempt not found." });

            // If timeLimit exists, check if the assessment has expired
            if (attempt.Assessment.Time_Limit.HasValue)
            {
                var endTime = attempt.Assessment.Starting_At.AddMinutes(attempt.Assessment.Time_Limit.Value);
                if (DateTime.UtcNow > endTime)
                    return BadRequest(new { message = "Cannot submit. The assessment has expired." });
            }

            var result = await _service.SubmitAttemptAsync(attemptId, responses ?? new());
            if (result is null) return NotFound(new { message = "Attempt not found." });
            return Ok(result);
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetMyAttempts()
        {
            var studentId = await GetStudentIdAsync();
            if (studentId is null) return Unauthorized(new { message = "Student profile not found." });

            var attempts = await _service.GetAttemptsByStudentAsync(studentId.Value);
            return Ok(attempts);
        }
    }
}