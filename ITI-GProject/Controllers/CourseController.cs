using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers

    {
        [ApiController]
        [Route("api/[controller]")]
        public class CourseController(ICourseService courseService, IStudentCoursesService studentCoursesService,AppGConetxt ctx) : ControllerBase
        {

        [Authorize(Roles = "Student,Assistance,Admin")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CourseDTO>>> GetAllCourses(
            [FromQuery] string? search,
            [FromQuery] CourseStatus? status,
            [FromQuery] string? category,
            [FromQuery] StudentYear? year,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)

        {

            if (User.IsInRole("Student"))
            {
                var userGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var myYear = await ctx.Students.AsNoTracking()
                     .Where(s => s.UserId == userGuid)
                     .Select(s => (StudentYear?)s.Year)
                     .FirstOrDefaultAsync();


                if (!myYear.HasValue)
                    return Forbid("?? ???? ??? ?????? ?????? ??????.");

                year = myYear;
            }

            var result = await courseService.GetAllCoursesAsync(
                search, status, category, year, page, pageSize);

            return Ok(result);
        }



        [Authorize(Roles = "Student,Assistance,Admin")]
        [HttpGet("{CourseId:int}")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int CourseId)
            {
                var courseDto = await courseService.GetCourseByIdAsync(CourseId);
                return courseDto is null ? NotFound("Course not found") : Ok(courseDto);
            }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseDTO>> CreateCourse([FromForm] CourseUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await courseService.CreateCourseAsync(dto);
            if (created is null) return BadRequest("Validation failed");

            return CreatedAtAction(nameof(GetCourseById), new { CourseId = created.Id }, created);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCourseById(int id)
        {
            var isDeleted = await courseService.DeleteCourseById(id);
            if (isDeleted)
            {
                return Ok(new { message = $"Course With Id {id} Deleted" });
            }
            return NotFound(new { message = $"Course with Id {id} not found" });
        }
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id, [FromForm] CourseUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await courseService.UpdateCourseAsync(id, dto);
            if (updated is null) return NotFound("Not Course Found");
            return Ok(updated);
        }

        [HttpGet("stats")]  
        public async Task<IActionResult> GetStats() => Ok(await courseService.GetStatsAsync());

        [HttpGet("{id:int}/content")]
        public async Task<IActionResult> GetContent(int id)
        {
            int? studentId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var sid)
                ? sid : (int?)null;

            var can = await studentCoursesService.CanAccessContentAsync(id, studentId, User);
            if (!can)
                return User.Identity?.IsAuthenticated == true ? Forbid() : Unauthorized();

            var content = await courseService.GetCourseContentAsync(id);
            return content is null ? NotFound() : Ok(content);
        }


    }
}
    
