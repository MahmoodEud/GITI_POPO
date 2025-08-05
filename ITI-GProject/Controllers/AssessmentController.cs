
namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {


        private readonly IAssessments _assessmentService;

        public AssessmentController(IAssessments assessmentsService)
        {
            _assessmentService = assessmentsService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssDTO>>> GetAllAssessments()
        {
            var AllAssessments = await _assessmentService.GetAllAssessments();
            return Ok(AllAssessments);
        }

        [HttpGet("{id}")]

        public async Task<ActionResult<AssDTO>>AssessmnetById(int id)
        {
            var assessment = await _assessmentService.GetAssessmetById(id);
            if (assessment == null)
            {
                return NotFound($"No Assessment with id : {id}");
            }

            return Ok(assessment);
        }

        [HttpGet("AssessmentByLesson/ {LessonId}")]

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
        public async Task<ActionResult<AssDTO>> CreateAssessment(
          [FromBody] CreateNewDTO createAssDTO,
          [FromQuery] List<QuesDTO> questions = null)
        {
            try
            {
                var assessment = await _assessmentService.CreateNewAssessment(createAssDTO, questions);
                return CreatedAtAction(nameof(AssessmnetById), new { id = assessment.Id }, assessment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
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
