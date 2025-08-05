namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(AppGConetxt context, UserManager<ApplicationUser> userManager,IConfiguration config, RoleManager<IdentityRole> roleManager
) : ControllerBase
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
              bool Found=await userManager.CheckPasswordAsync(userName, loginDto.Password);
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


    }
}
