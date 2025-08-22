using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _service;
        private readonly AppGConetxt _ctx;

        public NotificationsController(INotificationsService service, AppGConetxt ctx)
        {
            _service = service;
            _ctx = ctx;
        }
        private async Task<int?> GetStudentIdAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            var s = await _ctx.Students.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
            return s?.Id;
        }
        [HttpPost("broadcast")]
        [Authorize(Roles = "Admin,Assistance")]
        public async Task<ActionResult<object>> Broadcast([FromBody] BroadcastNotificationRequest req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Title))
                return BadRequest("Title is required.");

            var audience = await _service.ResolveAudienceAsync(req.Year, req.CourseId);
            if (audience.Count == 0)
                return Ok(new { count = 0, message = "No recipients matched the filters." });

            var affected = await _service.CreateManyAsync(
                audience,
                req.Title,
                req.Body ?? "",
                req.Type,
                req.ActionUrl
            );

            return Ok(new { count = affected });
        }
        [HttpPost("exam-now")]
        [Authorize(Roles = "Admin,Assistance")]
        public async Task<ActionResult<object>> ExamNow([FromBody] ExamNowRequest req)
        {
            if (req is null || req.Year <= 0 || req.CourseId <= 0 || req.LessonId <= 0)
                return BadRequest("Year, CourseId and LessonId are required.");

            var audience = await _service.ResolveAudienceAsync(req.Year, req.CourseId);
            if (audience.Count == 0)
                return Ok(new { count = 0, message = "No recipients matched the filters." });

            var title = string.IsNullOrWhiteSpace(req.Title)
                ? "امتحان بدأ الآن"
                : req.Title!;
            var body = string.IsNullOrWhiteSpace(req.Body)
                ? "اضغط للدخول على الامتحان."
                : req.Body!;

            var actionUrl = $"/quiz/{req.LessonId}";

            var affected = await _service.CreateManyAsync(
                audience, title, body, NotificationType.Exam, actionUrl);

            return Ok(new { count = affected });
        }
        [HttpPost("meeting")]
        [Authorize(Roles = "Admin,Assistance")]
        public async Task<ActionResult<object>> Meeting([FromBody] MeetingNotificationRequest req)
        {
            if (req is null || req.CourseId <= 0 || string.IsNullOrWhiteSpace(req.Title) || string.IsNullOrWhiteSpace(req.JoinUrl))
                return BadRequest("CourseId, Title and JoinUrl are required.");

            var audience = await _service.ResolveAudienceAsync(req.Year, req.CourseId);
            if (audience.Count == 0)
                return Ok(new { count = 0, message = "No recipients matched the filters." });

            var affected = await _service.CreateManyAsync(
                audience, req.Title, "اضغط للانضمام للاجتماع.", NotificationType.Meeting, req.JoinUrl);

            return Ok(new { count = affected });
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Assistance")]
        public async Task<ActionResult<NotificationDto>> Create([FromBody] NotificationCreateRequest req)
        {
            if (req is null || req.StudentId <= 0 || string.IsNullOrWhiteSpace(req.Title)) return BadRequest();

            var dto = await _service.CreateAsync(
                req.StudentId,
                req.Title,
                req.Body ?? "",
                req.Type,        
                req.ActionUrl     
            );

            return Ok(dto);
        }

        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> Mine([FromQuery] int? take)
        {
            var sid = await GetStudentIdAsync();
            if (sid == null) return Unauthorized();
            var list = await _service.GetByStudentAsync(sid.Value, take);
            return Ok(list);
        }
        [Authorize]
        [HttpGet("mine/unread-count")]
        public async Task<ActionResult<int>> UnreadCount()
        {
            var sid = await GetStudentIdAsync();
            if (sid == null) return Unauthorized();
            var n = await _service.GetUnreadCountAsync(sid.Value);
            return Ok(n);
        }

        [HttpPost("{id:int}/read")]
        public async Task<ActionResult> MarkRead(int id)
        {
            var sid = await GetStudentIdAsync();
            if (sid == null) return Unauthorized();
            var ok = await _service.MarkAsReadAsync(id, sid.Value);
            if (!ok) return NotFound();
            return Ok();
        }
        [Authorize]
        [HttpPost("read-all")]
        public async Task<ActionResult<int>> MarkAllRead()
        {
            var sid = await GetStudentIdAsync();
            if (sid == null) return Unauthorized();
            var count = await _service.MarkAllAsReadAsync(sid.Value);
            return Ok(count);
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Admin,Assistance")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> ByStudent(int studentId, [FromQuery] int? take)
        {
            var list = await _service.GetByStudentAsync(studentId, take);
            return Ok(list);
        }

    }
}