using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController(IStudentsService studentsService) : Controller
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetAllStudent()
        {
            var Students = await studentsService.GetAllStudentAsync();
            if (Students == null) return NotFound();

            return Ok(Students);
        }
        [HttpGet("{Id}")]
        public async Task<ActionResult<StudentDTO>> GetStudentById(string Id)
        {
            var Student = await studentsService.GetStudentByIdAsync(Id);

            if(Student == null) return NotFound();

            return Ok(Student);
        }
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteStudent (string Id)
        {
            var res = await studentsService.DeleteStudentAsync(Id);

            if(res is true) return Ok();

            return BadRequest();
        }

        [HttpPut("{Id}")]
        public async Task<ActionResult<StudentDTO>> UpdateStudent(string Id , UpdateStudentDTO updateStudent)
        {
            var student = await studentsService.UpdataStudentAsync(Id, updateStudent);

            if(student == null) return BadRequest();

            return Ok(student);
        }
        
    }
}
