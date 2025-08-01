using System.Threading.Tasks;

namespace ITI_GProject.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDTO>> GetAllCoursesAsync();

        Task<CourseDTO> GetCourseByIdAsync(int id,int courseId );
        Task<CourseDTO> CreateCourseAsync(CourseUpdateDTO courseUpdateDTO);
        
        Task<bool> DeleteCourseById(int id);


        Task<CourseDTO> UpdateCourseAsync(int id, CourseUpdateDTO courseUpdateDTO);
    }
}
