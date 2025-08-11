using System.Data;

namespace ITI_GProject.Services
{
    public class StudentsService(UserManager<ApplicationUser> userManager) : IStudentsService
    {


        public async Task<PagedResult<StudentDTO>> GetAllStudentAsync
            (StudentYear? year, string? roleFilter, int pageNumber = 1, int pageSize = 50)
        {
            var query = userManager.Users
                .Include(u => u.StudentProfile)
                .AsQueryable();
            if (year.HasValue)
                query = query.Where(u => u.StudentProfile.Year == year.Value);

            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                roleFilter = roleFilter.Trim();
                var roleUserIds = await userManager.GetUsersInRoleAsync(roleFilter);
                var filteredIds = roleUserIds.Select(u => u.Id).ToHashSet();
                query = query.Where(u => filteredIds.Contains(u.Id));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var students = await query
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync();
            //var students = await userManager.Users
            //    .Include(u => u.StudentProfile)
            //    .ToListAsync();


            var studentDtos = new List<StudentDTO>();

            foreach (var user in students)
            {
                var roles = await userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? string.Empty;

                studentDtos.Add(new StudentDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    StudentLevel = (user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي).ToString(),
                    ParentNumber = user.StudentProfile?.ParentNumber ?? string.Empty,
                    ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
                    Birthdate = user.Birthdate,
                    Year = user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي,
                    Role = role
                });
            }

            return new PagedResult<StudentDTO>
            {
                Items = studentDtos,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }

        public async Task<StudentDTO> GetStudentByIdAsync(string Id)
        {
            var user = await userManager.Users
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync(u => u.Id == Id);
            if (user is null) return null!;
            //var user = await userManager.FindByIdAsync(Id);
            var role = (await userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;

            return new StudentDTO()
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                StudentLevel = (user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي).ToString(),
                ParentNumber = user.StudentProfile?.ParentNumber ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl ?? string.Empty,
                Birthdate = user.Birthdate,
                Year = user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي,
                Role = role
            };

        }

        public async Task<bool> DeleteStudentAsync(string Id)
        {
            if (string.IsNullOrWhiteSpace(Id))
                return false;

            var user = await userManager.FindByIdAsync(Id);
            if (user is null) return false;

            var res = await userManager.DeleteAsync(user);
            return res.Succeeded;
        }

        public async Task<UpdateStudentDTO> UpdateStudentAsync(string Id, UpdateStudentDTO updateStudent)
        {
            if (string.IsNullOrWhiteSpace(Id)) return null;
            var user = await userManager.Users
              .Include(u => u.StudentProfile)
              .FirstOrDefaultAsync(u => u.Id == Id);

            if (user is null) return null!;
            user.Name = updateStudent.Name;
            user.UserName = updateStudent.UserName ?? user.UserName;
            user.PhoneNumber = updateStudent.PhoneNumber;
            user.StudentProfile.Year = updateStudent.Year;
            user.Birthdate = updateStudent.Birthdate ?? DateTime.MinValue;

            if (user.StudentProfile != null)
            {
                user.StudentProfile.ParentNumber = updateStudent.ParentNumber ?? user.StudentProfile.ParentNumber;
            }


            var res = await userManager.UpdateAsync(user);
            if (res.Succeeded)
            {

                return new UpdateStudentDTO
                {
                    Name = user.Name,
                    UserName = user.UserName,
                    Year =user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي,
                    PhoneNumber = user.PhoneNumber,
                    ParentNumber = user.StudentProfile?.ParentNumber ?? string.Empty,
                    Birthdate = user.Birthdate,
                };
            }

            return null!;
            //Year = user.StudentProfile?.Year ?? StudentYear.الصف_الأول_الثانوي,

        }

        public async Task<DashboardStatsDTO> GetDashboardStatsAsync()
        {
            var students = await userManager.Users
                .Include(u => u.StudentProfile)
                .ToListAsync();

            var admins = await userManager.GetUsersInRoleAsync("Admin");
            var assistants = await userManager.GetUsersInRoleAsync("Assistance");

            return new DashboardStatsDTO
            {
                TotalStudents = students.Count,
                FirstYearStudents = students.Count(s => s.StudentProfile?.Year == StudentYear.الصف_الأول_الثانوي),
                SecondYearStudents = students.Count(s => s.StudentProfile?.Year == StudentYear.الصف_الثاني_الثانوي),
                ThirdYearStudents = students.Count(s => s.StudentProfile?.Year == StudentYear.الصف_الثالث_الثانوي),
                TotalAdmins = admins.Count,
                TotalAssistants = assistants.Count
            };
        }

    }
}
