using Microsoft.AspNetCore.Authorization;

    namespace ITI_GProject.Controllers

    {
        [ApiController]
        [Route("api/[controller]")]
        public class CourseController(ICourseService courseService) : ControllerBase
        {
        [Authorize]
        [HttpGet]
            public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCourses()
            {
                var Courses = await courseService.GetAllCoursesAsync();

                if (Courses is not null)
                {
                    return Ok(Courses);

                }

                return NotFound("Not Course Found");

            }

            //[Authorize]
            [HttpGet("{CourseId:int}")]
            public async Task<ActionResult<CourseDTO>> GetCourseById(int CourseId)
            {

                var userRole = User.FindFirstValue(ClaimTypes.Role);

                if (userRole == "Admin")
                {
                    var courseDto = await courseService.GetCourseByIdAsync(CourseId);
                    if (courseDto is null) return NotFound("Not Course Found");
                    return Ok(courseDto);
                }

                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int studentId);
                if (studentId == 0)
                {
                    return Unauthorized("Student is not logged in.");
                }
                var CourseDTO = await courseService.GetCourseByIdAsync(CourseId, studentId);
                if (CourseDTO is not null)
                {
                    return Ok(CourseDTO);
                }
                return NotFound();
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
        }
}
    
