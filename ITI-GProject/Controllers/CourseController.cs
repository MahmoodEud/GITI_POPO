using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers

    {
        [ApiController]
        [Route("api/[controller]")]
        public class CourseController(ICourseService courseService, IStudentCoursesService studentCoursesService) : ControllerBase
        {

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CourseDTO>>> GetAllCourses(
            [FromQuery] string? search,
            [FromQuery] CourseStatus? status,
            [FromQuery] string? category,
            [FromQuery] string? level,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await courseService.GetAllCoursesAsync(search, status, category, page, pageSize);
            return Ok(result);

        }

            //[Authorize]
        [HttpGet("{CourseId:int}")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int CourseId)
            {
                var courseDto = await courseService.GetCourseByIdAsync(CourseId);
                return courseDto is null ? NotFound("Course not found") : Ok(courseDto);
            }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CourseDTO>> CreateCourse([FromForm] CourseUpdateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await courseService.CreateCourseAsync(dto);
            if (created is null) return BadRequest("Validation failed");

            return CreatedAtAction(nameof(GetCourseById), new { CourseId = created.Id }, created);
        }


        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCourseById(int id)
        {
            var isDeleted = await courseService.DeleteCourseById(id);
            if (isDeleted)
            {
                return Ok($"Course With Id{id} Id Deleted");
            }
            return NotFound();
        }

        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")] 
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
    
