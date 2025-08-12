using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentsService studentsService) : ControllerBase
    {
        //[Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<StudentDTO>>> GetAllStudent(StudentYear? year, [FromQuery] string? roleFilter, int pageNumber = 1, int pageSize = 50)
        {
            var result = await studentsService.GetAllStudentAsync(year, roleFilter, pageNumber, pageSize);

            if (!result.Items.Any())
                return NotFound("مفيش اي مستخدمين راجعين ف الوقت الحالي");

            return Ok(result);
        }



        [HttpGet("statistics")]
        public async Task<ActionResult<DashboardStatsDTO>> GetDashboardStats() {
            var state = await studentsService.GetDashboardStatsAsync();
            return Ok(state);
        }


        [HttpGet("{Id}")]
        public async Task<ActionResult<StudentDTO>> GetStudentById(string Id)
        {
            var Student = await studentsService.GetStudentByIdAsync(Id);

            if (Student == null) return NotFound("لا يوجد اي طالب بهذا الرقم");

            return Ok(Student);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteStudent(string Id)
        {
            var res = await studentsService.DeleteStudentAsync(Id);

            if (res is true) return Ok();

            return BadRequest();
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<StudentDTO>> UpdateStudent(string Id, UpdateStudentDTO updateStudent)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var student = await studentsService.UpdateStudentAsync(Id, updateStudent);

            if (student == null) return BadRequest("الطالب غير موجود");

            return Ok(student);
        }

    }
}
