namespace ITI_GProject.Services
{
    public interface IStudentsService
    {
        Task<PagedResult<StudentDTO>> GetAllStudentAsync(StudentYear? year, string? roleFilter, int pageNumber = 1, int pageSize = 50);
        Task<StudentDTO> GetStudentByIdAsync(string Id);

        Task<bool> DeleteStudentAsync(string Id);
        Task<UpdateStudentDTO> UpdateStudentAsync(string Id, UpdateStudentDTO updateStudent);
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
    }
}
