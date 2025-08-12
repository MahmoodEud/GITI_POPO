using System.Threading.Tasks;

namespace ITI_GProject.Services
{
    public interface ICourseService
    {

         Task<PagedResult<CourseDTO>> GetAllCoursesAsync(
            string? search = null,
            CourseStatus? status = null,
            string? category = null,
            int page = 1,
            int pageSize = 20);

        Task<CourseDTO?> GetCourseByIdAsync(int courseId);
        Task<CourseDTO?> CreateCourseAsync(CourseUpdateDTO courseUpdateDTO);
        Task<CourseStatsDTO> GetStatsAsync();
        
        Task<bool> DeleteCourseById(int id);

        Task<CourseContentDTO?> GetCourseContentAsync(int courseId);

        Task<CourseDTO> UpdateCourseAsync(int id, CourseUpdateDTO courseUpdateDTO);
    }
}
