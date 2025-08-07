using ITI_GProject.Services;
using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(ICourseService courseService ) : ControllerBase
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

        [Authorize]
        [HttpGet("CourseId")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int CourseId)
        {

            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if(userRole == "Admin")
            {
                var courseDto = await courseService.GetCourseByIdAsync(CourseId);
                return Ok(courseDto);
            }

            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int studentId);
            if (studentId == 0)
            {
                return Unauthorized("Student is not logged in.");
            }
            var CourseDTO = await courseService.GetCourseByIdAsync( CourseId ,studentId);
            if (CourseDTO is not null)
            {
                return Ok(CourseDTO);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CourseDTO>> CreateCourse([FromForm] CourseUpdateDTO CoursCreate)
        {
            var CourseCreate = await courseService.CreateCourseAsync( CoursCreate);

            if(CoursCreate is null)
            {
                return BadRequest();
            }

            return Ok(CoursCreate);

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteCourseById(int id)
        {
            var isDeleted = await courseService.DeleteCourseById(id);
            if(isDeleted)
            {
                return Ok($"Course With Id{id} Id Deleted");
            }
            return NotFound();
        }

        [HttpPut("id")]
        [Authorize]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id , CourseUpdateDTO courseUpdateDTO)
        {
            var CourseUpdate = await courseService.UpdateCourseAsync(id, courseUpdateDTO);

            if(CourseUpdate is null)
            {
                return BadRequest();
            }

            return Ok(CourseUpdate);
         }
    }
}
