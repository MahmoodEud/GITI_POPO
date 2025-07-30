using ITI_GProject.DTOs.ChoicesDTO;
using ITI_GProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChoiceController : ControllerBase
    {
        private readonly IChoice _choiceService;
        private readonly IMapper _mapper;
        public ChoiceController(IChoice choiceService, IMapper mapper)
        {
            _choiceService = choiceService;
            _mapper = mapper;
        }


        [HttpGet("GetChoiceById/{id}")]

        public async Task<ActionResult<ChoiceDTO>>GetChoiceById (int id)
        {

            try
            {
                var choice = await _choiceService.GetChoiceById(id);
                return Ok(choice);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }


        }

        [HttpGet("GetByQuestionId/{questionId}")]

        public async Task<ActionResult<IEnumerable<ChoiceDTO>>>GetChoiceByQuestionId(int questionId)
        {
            var choices = await _choiceService.GetChoicesByQuestion(questionId);
            return Ok(choices);

        }


        [HttpPost]

        public async Task<ActionResult<ChoiceDTO>>AddNewChoice(CreateDTO dto)
        {
            try
            {
                var choice = await _choiceService.CreateChoice(dto);
                return Ok(choice);

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]

        public async Task<ActionResult> UpdateChoice(int id, UpdateDTO updateDto)
        {
            try
            {


                var updated = await _choiceService.UpdateChoice(id, updateDto);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("id")]
        public async Task<IActionResult>DeleteChoice (int id)
        {
            try
            {
                  await _choiceService.DeleteChoice(id);
                return Ok(new { message = "Choice deleted successfully !" });

            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);

            }

        }
    }
}
