using ITI_GProject.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.Numerics;

namespace ITI_GProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(AppGConetxt context,
    UserManager<ApplicationUser> userManager,
    IConfiguration config,
    RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromForm] RegisterDTO registerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await userManager.FindByNameAsync(registerDto.UserName) != null)
        {
            ModelState.AddModelError("UserName", "اسم المستخدم مستخدم بالفعل");
            return BadRequest(ModelState);
        }

        if (registerDto.Phone == registerDto.ParentPhone)
        {
            ModelState.AddModelError("ParentPhone", "رقم ولي الأمر لا يمكن أن يكون نفس رقم الطالب");
            return BadRequest(ModelState);
        }

        var file = registerDto.ProfilePicture;

        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("ProfilePicture", "الصورة مطلوبة");
            return BadRequest(ModelState);
        }

        if (file.Length > 2 * 1024 * 1024)
        {
            ModelState.AddModelError("ProfilePicture", "الحد الأقصى لحجم الصورة هو 2 ميجا");
            return BadRequest(ModelState);
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            ModelState.AddModelError("ProfilePicture", "نوع الصورة غير مدعوم. استخدم jpg أو png فقط");
            return BadRequest(ModelState);
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var imagePath = Path.Combine("wwwroot/images/profiles", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(imagePath)!);

        using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var user = new ApplicationUser
        {
            UserName = registerDto.UserName,
            Name = registerDto.Name,
            PhoneNumber = registerDto.Phone,
            Birthdate = registerDto.Birthdate,
            ProfilePictureUrl = $"/images/profiles/{fileName}"
        };

        using var transaction = await context.Database.BeginTransactionAsync();

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return BadRequest(ModelState);
        }

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
        await transaction.CommitAsync();

        return Ok(new { message = "تم التسجيل بنجاح" ,
            profileImage = user.ProfilePictureUrl
        });
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
            var student = await context.Students.FirstOrDefaultAsync(s => s.UserId == userName.Id);

            bool Found =await userManager.CheckPasswordAsync(userName, loginDto.Password);
            if (Found)
            {
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
                    id = userName.Id,
                    username = userName.UserName,
                    name = userName.Name,
                    phone = userName.PhoneNumber,
                    profileImage = userName.ProfilePictureUrl,
                    studentYear = student?.Year.ToString(),
                    parentPhoneNumber = student?.ParentNumber,
                    role = roles?.FirstOrDefault()

                });
            }
            ModelState.AddModelError("Password", "اسم المستخدم او كلمة المرور خطأ حاول تاني ");
        }
        return BadRequest(ModelState);
    }

    // TODO(Mazen): Why is this method synchronous? Do we even use it?!!
    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = roleManager.Roles
            .Select(r => r.Name)
            .ToList();
        return Ok(roles);
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

    // POST: api/account/student-change-password
    [Authorize(Roles = "Student")]
    [HttpPost("student-change-password")]
    public async Task<IActionResult> StudentChangePassword([FromBody]UserChangePasswordDto request)
    {
        // get current logged-in user
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (result.Succeeded)
        {
            return Ok("Password changed successfully");
        }

        return BadRequest(result.Errors);
    }

    // POST: api/account/admin-change-password/{id}
    [Authorize(Roles = "Admin")]
    [HttpPost("admin-change-password/{id}")]
    public async Task<IActionResult> AdminChangePassword(string id, [FromBody]AdminChangePasswordDto request)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return BadRequest("User not found.");
        }

        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
        {
            return BadRequest(removeResult.Errors);
        }

        var addResult = await userManager.AddPasswordAsync(user, request.NewPassword);
        if (addResult.Succeeded)
        {
            return Ok("Password reset successfully");
        }

        return BadRequest(addResult.Errors);
    }
}