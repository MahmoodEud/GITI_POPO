namespace ITI_GProject.Profiles
{
    public class CourseProfiles:Profile
    {
        public CourseProfiles()
        {
            CreateMap<Course, CourseDTO>().ReverseMap();
            CreateMap<CourseUpdateDTO, Course>().ReverseMap();
            // Lessons
            CreateMap<Lesson, LessonDTO>().ReverseMap();
            CreateMap<LessonUpdateDto, Lesson>().ReverseMap();
        }
    }
}
