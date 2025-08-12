using Microsoft.AspNetCore.Authorization;
[Route("api/student-courses")]
[ApiController]
[Authorize(Roles = "Student,Admin,Assistant")]
public class StudentCoursesController : ControllerBase
{
    private readonly IStudentCoursesService sxc;
    private readonly AppGConetxt _ctx;

    public StudentCoursesController(IStudentCoursesService sxc, AppGConetxt ctx)
    {
        this.sxc = sxc;
        _ctx = ctx;
    }

    private async Task<int?> GetStudentIdAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
        if (string.IsNullOrWhiteSpace(userId)) return null;

        var student = await _ctx.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId); 

        return student?.Id;
    }
    //Free Course
    [HttpPost("{courseId:int}")]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId is null) return Unauthorized("Student profile not found.");

        var result = await sxc.EnrollAsync(studentId.Value, courseId); 

        return result switch
        {
            EnrollResult.Success => Ok("تمام التسجيل في الكورس المجاني نجح"),
            EnrollResult.AlreadyEnrolled => BadRequest("أنت مسجل بالفعل"),
            EnrollResult.CourseNotFound => NotFound("الكورس غير موجود"),
            EnrollResult.PaidNotAllowed => Forbid("لا يمكنك الاشتراك في كورس مدفوع. تواصل مع الأدمن."),
            _ => BadRequest()
        };
    }
    //Delete Free Course
    [HttpDelete("Delete/{courseId:int}")]
    public async Task<IActionResult> Unenroll(int courseId)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId is null) return Unauthorized("Student profile not found.");

        return await sxc.UnenrollAsync(studentId.Value, courseId)
            ? Ok("تم إلغاء التسجيل")
            : NotFound("غير مسجل في هذا الكورس");
    }
    //My Couresesssssssss
    [HttpGet("my")]
    public async Task<IActionResult> GetMyCourses()
    {
        var studentId = await GetStudentIdAsync();
        if (studentId is null) return Unauthorized("Student profile not found.");

        var courses = await sxc.GetMyCoursesAsync(studentId.Value);
        return Ok(courses);
    }
    //content in Course
    [HttpGet("{courseId:int}/lesson")]
    // مفيش Authorize هنا؛ السماح/المنع بيحصل داخل CanAccessContentAsync
    public async Task<IActionResult> GetContent(int courseId, [FromServices] ICourseService courseService)
    {
        var studentId = await GetStudentIdAsync();

        var can = await sxc.CanAccessContentAsync(courseId, studentId, User);
        if (!can)
            return User.Identity?.IsAuthenticated == true ? Forbid() : Unauthorized();

        var content = await courseService.GetCourseContentAsync(courseId);
        return content is null ? NotFound("لا يوجد محتوى") : Ok(content);
    }
    public record AdminEnrollRequest(int StudentId, int CourseId);
    //Admin Add Student to Course !!!
    [HttpPost("admin/enroll")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminEnroll([FromBody] AdminEnrollRequest req)
    {
        var result = await sxc.EnrollAsync(req.StudentId, req.CourseId, allowPaid: true);

        return result switch
        {
            EnrollResult.Success => Ok("تم تسجيل الطالب في الكورس"),
            EnrollResult.AlreadyEnrolled => BadRequest("الطالب مسجّل بالفعل"),
            EnrollResult.CourseNotFound => NotFound("الكورس غير موجود"),
            _ => BadRequest()
        };
    }
    //Admin Can Delete Student from Course !!!
    [HttpDelete("admin/{courseId:int}/unenroll/{studentId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminUnenroll(int courseId, int studentId)
    {
        var ok = await sxc.UnenrollAsync(studentId, courseId);
        return ok ? Ok("تم إلغاء تسجيل الطالب من الكورس")
                  : NotFound("الطالب غير مسجّل في هذا الكورس أو الكورس غير موجود");
    }
    [HttpDelete("{idcou:int}")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Uninroll(int idcou)
    {
        var studentId = await GetStudentIdAsync();
        if (studentId is null) return Unauthorized("Student profile not found.");
        return await sxc.UnenrollAsync(studentId.Value, idcou)
            ? Ok("تم إلغاء التسجيل")
            : NotFound("غير مسجّل في هذا الكورس");
    }


}
