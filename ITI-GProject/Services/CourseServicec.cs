
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace ITI_GProject.Services
{
    public class CourseServices(AppGConetxt _context, IMapper _mapper) : ICourseService
    {
        public async Task<CourseDTO> CreateCourseAsync(CourseUpdateDTO courseUpdateDTO)
        {
            if (courseUpdateDTO == null)
            {
                return null!;
            }

            var course = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);

            await _context.Courses.AddAsync(course);

            int res = await _context.SaveChangesAsync();

            if (res > 0)
            {
                var courseDTo = _mapper.Map<Course, CourseDTO>(course);
                return courseDTo;
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

        public async Task<CourseDTO> GetCourseByIdAsync( int CourseId)
        {

            //var isEnrolled = await _context.StudentCourses.AnyAsync(s => s.Student_Id == studentId && s.Course_Id == CourseId);

            //if (!isEnrolled)
            //{
            //    return null!;
            //}



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
            if (course is null)
            {
                return null!;
            }

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
