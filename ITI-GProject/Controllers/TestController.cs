using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize(Roles ="Student")]
        [HttpGet]
        public IActionResult Get()
        {
            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(new
            {
                Message = "✅ تم الوصول بنجاح! التوكن شغال.",
                UserName = username,
                Role = role
            });
        }
        [Authorize(Roles = "Assistance")]
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok(new
            {
                Protocol = Request.Scheme,
                Host = Request.Host.ToString(),
                IsHttps = Request.IsHttps
            });
        }


        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TestController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
