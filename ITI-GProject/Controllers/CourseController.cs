using ITI_GProject.Services;

namespace ITI_GProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CourseController(ICourseService courseService ) : ControllerBase
    {

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

        [HttpGet("CourseId")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int CourseId)
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int studentId);
            var CourseDTO = await courseService.GetCourseByIdAsync( CourseId ,studentId);
            if (studentId == 0)
            {
                return Unauthorized("Student is not logged in.");
            }
            if (CourseDTO is not null)
            {
                return Ok(CourseDTO);
            }
            return NotFound();
        }

        [HttpPost]

        public async Task<ActionResult<CourseDTO>> CreateCourse(CourseUpdateDTO CoursCreate)
        {
            var CourseCreate = await courseService.CreateCourseAsync( CoursCreate);

            if(CoursCreate is null)
            {
                return BadRequest();
            }

            return Ok(CoursCreate);

        }

        [HttpDelete("{id}")]

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
