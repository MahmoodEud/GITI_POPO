using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,Admin,Assistant")]

    public class progressController : ControllerBase
    {
        private readonly IProgressService _svc;
        private readonly AppGConetxt _ctx;
        public progressController(IProgressService svc, AppGConetxt ctx)
        {
            _svc = svc; _ctx = ctx;
        }
        private async Task<int?> GetStudentIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            if (string.IsNullOrWhiteSpace(userId)) return null;
            var s = await _ctx.Students.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            return s?.Id; 
        }
        [HttpPut("lessons/{lessonId:int}")]
        public async Task<IActionResult> UpsertLesson(int lessonId, [FromBody] LessonProgressUpsertDTO dto)
        {
            var sid = await GetStudentIdAsync();
            if (sid is null && !User.IsInRole("Admin")) return Unauthorized();

            var studentId = sid ?? int.Parse(Request.Query["studentId"]);
            var result = await _svc.UpsertLessonAsync(studentId, lessonId, dto);
            return Ok(result);
        }
        [HttpGet("lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLesson(int lessonId)
        {
            var sid = await GetStudentIdAsync();
            if (sid is null && !User.IsInRole("Admin")) return Unauthorized();
            var studentId = sid ?? int.Parse(Request.Query["studentId"]);
            var p = await _svc.GetLessonAsync(studentId, lessonId);
            return p is null ? NotFound() : Ok(p);
        }
        [HttpGet("courses/{courseId:int}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var sid = await GetStudentIdAsync();
            if (sid is null && !User.IsInRole("Admin")) return Unauthorized();
            var studentId = sid ?? int.Parse(Request.Query["studentId"]);
            var p = await _svc.GetCourseAsync(studentId, courseId);

            if (p is null)
            {
                return Ok(new CourseProgressDTO
                {
                    CourseId = courseId,
                    TotalLessons = 0,
                    CompletedLessons = 0,
                    Percent = 0,
                    Lessons = new List<LessonProgressDTO>()
                });
            }

            return Ok(p);
        }

    }
}
