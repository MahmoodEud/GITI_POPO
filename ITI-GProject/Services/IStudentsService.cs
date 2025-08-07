namespace ITI_GProject.Services
{
    public interface IStudentsService
    {
         Task<IEnumerable<StudentDTO>> GetAllStudentAsync();

        Task<StudentDTO> GetStudentByIdAsync(string Id);

        Task<bool> DeleteStudentAsync(string Id);
        Task<StudentDTO> UpdataStudentAsync(string Id, UpdateStudentDTO updateStudent);
    }
}
