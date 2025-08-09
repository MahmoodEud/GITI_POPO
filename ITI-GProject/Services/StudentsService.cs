using Microsoft.AspNetCore.Identity;

namespace ITI_GProject.Services
{
    public class StudentsService(UserManager<ApplicationUser> userManager) : IStudentsService
    {

        public async Task<IEnumerable<StudentDTO>> GetAllStudentAsync()
        {
            var Students = await userManager.GetUsersInRoleAsync("Student");

            if(Students is null)
            {
                return null!;
            }
            var StudentsDTo = Students.Select(U => new StudentDTO
            {
                Id = U.Id,
                Name = U.Name,
                PhoneNumber = U.PhoneNumber ?? "",
                Password = "",
                StudentLevel = U.StudentProfile.Year.ToString()?? "",
                ParentNumber = U.StudentProfile.ParentNumber




            }).ToList();

            return StudentsDTo;
        }

        public async Task<StudentDTO> GetStudentByIdAsync(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);

            if (user is null) return null!;

            var isStudent = await userManager.IsInRoleAsync(user, "Student");
            if (!isStudent) return null!;

            return new StudentDTO()
            {
                Id = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber ?? "",
                Password = "",
                StudentLevel = user.StudentProfile?.Year.ToString() ?? "",
                ParentNumber = user.StudentProfile?.ParentNumber ?? ""
            };

        }

        public async Task<bool> DeleteStudentAsync(string Id)
        {
            if (Id == null) return false;


            var user = await userManager.FindByIdAsync(Id);
            if (user is null) return false;

            var isStudent = await userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
                return false;

           var res = await userManager.DeleteAsync(user);
            return res.Succeeded;
        }

        public async Task<StudentDTO> UpdataStudentAsync(string Id , UpdateStudentDTO updateStudent )
        {
            if (Id == null) return null!;


            var user = await userManager.FindByIdAsync(Id);
            if (user is null) return null!;

            var isStudent = await userManager.IsInRoleAsync(user, "Student");
            if (!isStudent)
                return null!;

            user.Name = updateStudent.Name;
            user.PhoneNumber = updateStudent.PhoneNumber;
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await userManager.ResetPasswordAsync(user, token, updateStudent.Password);

            var res = await userManager.UpdateAsync(user);
            if(res.Succeeded)
            {

                return new StudentDTO()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Password = updateStudent.Password,
                    PhoneNumber = user.PhoneNumber
                };
            }

            return null!;

        }
    }
}
