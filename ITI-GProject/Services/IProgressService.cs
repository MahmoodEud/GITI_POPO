namespace ITI_GProject.Services
{
    public interface IProgressService
    {
     
            Task<LessonProgressDTO> UpsertLessonAsync(int studentId, int lessonId, LessonProgressUpsertDTO dto);
            Task<LessonProgressDTO?> GetLessonAsync(int studentId, int lessonId);
            Task<CourseProgressDTO?> GetCourseAsync(int studentId, int courseId);
        
    }
}
