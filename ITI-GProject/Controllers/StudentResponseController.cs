using ITI_GProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentResponseController : ControllerBase
    {
            private readonly IStudentResponseService _service;

            public StudentResponseController(IStudentResponseService service)
            {
                _service = service;
            }

            [HttpPost("attempt/{attemptId:int}")]
            [Authorize(Roles = "Student,Admin,Assistant")]
            public async Task<IActionResult> AddResponse(int attemptId, [FromBody] StudentResponseCreateDTO dto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var created = await _service.AddResponseAsync(attemptId, dto);
                if (created is null)
                    return NotFound(new { message = "Attempt/Question/Choice not found or not related." });

                return Ok(new { message = "تم حفظ الإجابة / Response saved", data = created });
            }

            [HttpGet("attempt/{attemptId:int}")]
            public async Task<IActionResult> GetByAttempt(int attemptId)
            {
                var list = await _service.GetResponsesByAttemptAsync(attemptId);
                if (!list.Any())
                    return NotFound(new { message = "لا توجد إجابات لهذه المحاولة / No responses for this attempt." });

                return Ok(new { message = "تم استرجاع الإجابات / Responses retrieved", data = list });
            }

            [HttpGet("{id:int}")]
            public async Task<IActionResult> GetOne(int id)
            {
                var res = await _service.GetResponseAsync(id);
                if (res is null)
                    return NotFound(new { message = "Response not found." });

                return Ok(new { message = "تم الاسترجاع / Retrieved", data = res });
            }

            [HttpDelete("{id:int}")]
            [Authorize(Roles = "Admin,Assistant")]
            public async Task<IActionResult> Delete(int id)
            {
                var ok = await _service.DeleteResponseAsync(id);
                if (!ok)
                    return NotFound(new { message = "Response not found." });

                return Ok(new { message = "تم الحذف / Deleted" });
            }
        }
    }
