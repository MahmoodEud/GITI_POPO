namespace ITI_GProject.Services
{
    public enum EnrollResult { Success, AlreadyEnrolled, CourseNotFound, PaidNotAllowed }

    public interface IStudentCoursesService
    {
        Task<EnrollResult> EnrollAsync(int studentId, int courseId, bool allowPaid = false);
        Task<bool> UnenrollAsync(int studentId, int courseId);
        Task<IReadOnlyList<CourseDTO>> GetMyCoursesAsync(int studentId);
        Task<bool> CanAccessContentAsync(int courseId, int? studentId, ClaimsPrincipal user);

    }
}
