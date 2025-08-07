
using ITI_GProject.Services.Attachment;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace ITI_GProject.Services
{
    public class CourseServices(AppGConetxt _context, IMapper _mapper , IAttachment attachment) : ICourseService
    {
        public async Task<CourseDTO> CreateCourseAsync([FromForm] CourseUpdateDTO courseUpdateDTO)
        {
            if (courseUpdateDTO == null)
                return null!;

            var course = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);

            
            if (courseUpdateDTO.PicturalUrl != null && courseUpdateDTO.PicturalUrl.Length > 0)
            {
                var uploadedPath = attachment.Uplaod(courseUpdateDTO.PicturalUrl, "Image");
                course.PicturalUrl = uploadedPath ?? "";  
            }
            else
            {
               course.PicturalUrl = "default.jpg"; 
            }

            await _context.Courses.AddAsync(course);
            int res = await _context.SaveChangesAsync();

            if (res > 0)
            {
                var courseDto = _mapper.Map<Course, CourseDTO>(course);
                return courseDto;
            }

            return null!;
        }


        public async Task<bool> DeleteCourseById(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return false;
            }
            _context.Courses.Remove(course);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesAsync()
        {
            var Coursers = await _context.Courses.ToListAsync();
            if (Coursers is null || Coursers.Count == 0)
            {
                return null!;
            }
            var coursesDTO = _mapper.Map<List<Course>, List<CourseDTO>>(Coursers);
            return coursesDTO;
        }
        public async Task<CourseDTO> GetCourseByIdAsync(int CourseId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == CourseId);
            if (course == null)
            {
                return null!;
            }
            var courseDTO = _mapper.Map<Course, CourseDTO>(course);

            return courseDTO;
        }
        public async Task<CourseDTO> GetCourseByIdAsync( int CourseId , int studentId)
        {

            var isEnrolled = await _context.StudentCourses.AnyAsync(s => s.Student_Id == studentId && s.Course_Id == CourseId);

            if (!isEnrolled)
            {
                return null!;
            }



            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == CourseId);
            if (course == null)
            {
                return null!;
            }
            var courseDTO = _mapper.Map<Course, CourseDTO>(course);

            return courseDTO;
        }

        public async Task<CourseDTO> UpdateCourseAsync(int id, CourseUpdateDTO courseUpdateDTO)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course is null || courseUpdateDTO is null)
            {
                return null!;
            }
            if(courseUpdateDTO.PicturalUrl is not null)
            {
                attachment.Delete(course.PicturalUrl);

            }
            course.PicturalUrl = attachment.Uplaod(courseUpdateDTO.PicturalUrl!, "Image");
            
            course.Title = courseUpdateDTO.Title;
            course.Description = courseUpdateDTO.Description;
            course.Category = courseUpdateDTO.Category;
            course.Year = courseUpdateDTO.Year;
            course.Status = courseUpdateDTO.Status;



            await _context.SaveChangesAsync();

            var courseDTo = _mapper.Map<Course, CourseDTO>(course);

            return courseDTo;
        }

   
    }
}
