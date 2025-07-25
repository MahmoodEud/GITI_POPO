

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(UserManager<ApplicationUser> userManager,IConfiguration config, RoleManager<IdentityRole> roleManager
) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterDTO registerDto) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userManager.Users.Any(u => u.UserName == registerDto.UserName))
            {
                ModelState.AddModelError("UserName", "اسم المستخدم مستخدم بالفعل");
                return BadRequest(ModelState);
            }

            ApplicationUser user = new ApplicationUser {
                UserName = registerDto.UserName,
                Name = registerDto.Name,
                Pphone = registerDto.Pphone,
                PhoneNumber = registerDto.Phone,
                Birthdate = registerDto.Birthdate,
                StudentYear = registerDto.studentYear,
                IsApproved = false,
            };
                var result =await userManager.CreateAsync(user,registerDto.Password);
                if (result.Succeeded)
                {
                    return Ok(new { message = "تم التسجيل بنجاح، في انتظار موافقة الإدارة" });
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            
            return BadRequest(ModelState);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto) {
            if (ModelState.IsValid)
            {
              ApplicationUser  userName=await userManager.FindByNameAsync(loginDto.UserName);
                if (userName == null) {
                    ModelState.AddModelError("Password", "اسم المستخدم او كلمة المرور خطأ حاول تاني ");
                    return BadRequest(ModelState);
                }
              bool Found=await userManager.CheckPasswordAsync(userName, loginDto.Password);
                if (Found) 
                {
                    //genrate token here
                
                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, userName.UserName));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier,userName.Id));
                    IList<string>result=  await userManager.GetRolesAsync(userName);
                    foreach (var item in result)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item));
                    }

                    var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));
                    var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //Design Token
                    JwtSecurityToken token = new JwtSecurityToken(
                        issuer: config["JWT:ValidIssuer"],
                        audience: config["JWT:ValidAudience"],
                        expires: DateTime.Now.AddDays(5),
                        claims: claims,
                        signingCredentials: signingCredentials
                        );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = DateTime.Now.AddDays(5),
                        username = userName.UserName

                    });
                }
                ModelState.AddModelError("Password", "اسم المستخدم او كلمة المرور خطأ حاول تاني ");
            }
                    return BadRequest(ModelState);



        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(string userName, string role)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null) return NotFound("User not found");

            if (!await roleManager.RoleExistsAsync(role))
                return BadRequest("Role does not exist");

            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return BadRequest("Failed to assign role");

            return Ok("Role assigned successfully");
        }

    }
}
