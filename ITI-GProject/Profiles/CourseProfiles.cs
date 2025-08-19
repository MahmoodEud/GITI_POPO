namespace ITI_GProject.Profiles
{
    public class CourseProfiles:Profile
    {
        public CourseProfiles()
        {

            // Lessons
            //CreateMap<Lesson, LessonDTO>().ReverseMap();
            CreateMap<LessonUpdateDto, Lesson>().ReverseMap();
            CreateMap<Lesson, LessonDTO>()
                .ForMember(d => d.videoUrl, o => o.MapFrom(s => s.VideoUrl))
                .ForMember(d => d.previewVideoUrl, o => o.MapFrom(s => s.PreviewVideoUrl))
                .ForMember(d => d.pdfUrl, o => o.MapFrom(s => s.PdfUrl));

            CreateMap<Course, CourseDTO>();
            CreateMap<CourseUpdateDTO, Course>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore())
                .ForMember(d => d.PicturalUrl, o => o.Ignore())
                .ForMember(d => d.CreatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore());
        }
    }
}
