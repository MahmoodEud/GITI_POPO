namespace ITI_GProject.Profiles
{
    public class CourseProfiles:Profile
    {
        public CourseProfiles()
        {
            CreateMap<Course, CourseDTO>().ReverseMap();

            CreateMap<CourseUpdateDTO, Course>()
                .ForMember(d => d.PicturalUrl, opt => opt.Ignore());
            // Lessons
            CreateMap<Lesson, LessonDTO>().ReverseMap();
            CreateMap<LessonUpdateDto, Lesson>().ReverseMap();
        }
    }
}
