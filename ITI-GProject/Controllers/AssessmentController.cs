
using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssessmentController : ControllerBase
    {

        private readonly IAssessments _assessmentService;
        
        public AssessmentController(IAssessments assessmentsService)
        {
            _assessmentService = assessmentsService;
        }

        
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssDTO>>> GetAllAssessments()
        {
            var AllAssessments = await _assessmentService.GetAllAssessments();
            return Ok(AllAssessments);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<AssDTO>>AssessmnetById(int id)
        {
            var assessment = await _assessmentService.GetAssessmetById(id);
            if (assessment == null)
            {
                return NotFound($"No Assessment with id : {id}");
            }

            return Ok(assessment);
        }

        [HttpGet("AssessmentByLesson/{LessonId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AssDTO>>> GetAssessmentsByLesson(int lessonId)
        {
            var assessments = await _assessmentService.GetAssessmentByLessonId(lessonId);

            if (!assessments.Any())
            {
                return NotFound($"No assessments found for lesson ID : {lessonId}");
            }

            return Ok(assessments);
         }

        [HttpPost]
        [Authorize(Roles = "Admin,Assistant")]
        public async Task<ActionResult<AssDTO>> CreateAssessment([FromBody] CreateNewDTO dto)
        {
            try
            {
                var assessment = await _assessmentService.CreateNewAssessment(dto);
                return CreatedAtAction(nameof(AssessmnetById), new { id = assessment.Id }, assessment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("lesson/{lessonId}")]
        [Authorize]
        public async Task<ActionResult<AssDTO>> GetByLesson(int lessonId)
        {
            var list = await _assessmentService.GetAssessmentByLessonId(lessonId);
            var first = list.FirstOrDefault();
            if (first == null) return NotFound($"No assessments found for lesson ID : {lessonId}");
            return Ok(first);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Assistant")]
        public async Task<IActionResult> DeleteAssessment(int id)
        {
            var result = await _assessmentService.DeleteAssessment(id);

            if (!result)
            {
                return NotFound();
            }

            return Ok("Deleted Successfully ");
        }





    }
}
