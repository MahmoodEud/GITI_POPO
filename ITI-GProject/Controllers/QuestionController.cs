using ITI_GProject.DTOs.QuestionsDTO;
using ITI_GProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestion _questionService;
        private readonly IMapper _mapper;

        public QuestionController(IQuestion questionService , IMapper mapper)
        {
            _questionService=questionService;
            _mapper=mapper;
        }



        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult>GetQuestion(int id)
        {
            try
            {
                var question = await _questionService.GetQuestionById(id);
                return Ok(question);

            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("GetByQuiz/{quizId}")]
        [Authorize]
        public async Task<ActionResult>GetQuestionByQuiz(int quizId)
        {
            var question = await _questionService.GetQuestionByQuizId(quizId);
            return Ok(question);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<QuesDTO>>CreateQustion(QuesDTO quesDTO)
        {
            try
            {
                var AddQuestion = await _questionService.CreateNewQuestion(quesDTO);
                return CreatedAtAction(nameof(GetQuestion), new { id = AddQuestion.Id }, AddQuestion);

            }
            catch (Exception ex) 
            {

                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult>UpdateQuestion(int id, [FromBody] QuesDTO quesDTO)
        {
            try
            {
                var updateQuestion = await _questionService.UpdateQuestion(id, quesDTO);
                return Ok(updateQuestion);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch(Exception ex)
            {
                return BadRequest( new { message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {

            var deleted = await _questionService.DeletQuestion(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();

        }



        

        
        

    }
}
