using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(AppGConetxt context, UserManager<ApplicationUser> userManager,IConfiguration config, RoleManager<IdentityRole> roleManager
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
                PhoneNumber = registerDto.Phone,
                Birthdate = registerDto.Birthdate,
                IsApproved = false,
            };
                var result =await userManager.CreateAsync(user,registerDto.Password);
                if (result.Succeeded)
                {
                var student = new Student
                {
                    Year = registerDto.studentYear,
                    PhoneNumber = registerDto.Phone,
                    ParentNumber = registerDto.ParentPhone,
                    IsActive = true,
                    UserId = user.Id
                };

                context.Students.Add(student);
                await context.SaveChangesAsync();

                await userManager.AddToRoleAsync(user, "Student");

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
                    if (!userName.IsApproved)
                    {
                        ModelState.AddModelError("Approval", "لم تتم الموافقة على الحساب بعد");
                        return BadRequest(ModelState);
                    }
                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, userName.UserName));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier,userName.Id));
                    var roles=  await userManager.GetRolesAsync(userName);
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                        claims.Add(new Claim("role", role));
                    }

                    var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));
                    var signingCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    //Design Token
                    JwtSecurityToken token = new JwtSecurityToken(
                        expires: DateTime.Now.AddDays(5),
                        claims: claims,
                        signingCredentials: signingCred
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
            if (user == null)
                return NotFound($"المستخدم '{userName}' غير موجود");

            if (!await roleManager.RoleExistsAsync(role))
                return BadRequest($"الدور '{role}' غير موجود");

            if (await userManager.IsInRoleAsync(user, role))
                return BadRequest($"المستخدم '{userName}' يمتلك الدور '{role}' بالفعل");

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return BadRequest($"فشل في تعيين الدور: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return Ok($"تم تعيين الدور '{role}' للمستخدم '{userName}' بنجاح");
        }

        [HttpPost("ApproveUser")]
        public async Task<IActionResult> ApproveUser(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("المستخدم غير موجود");

            user.IsApproved = true;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("فشل في تفعيل المستخدم");

            return Ok("تمت الموافقة على الحساب بنجاح");
        }

    }
}
