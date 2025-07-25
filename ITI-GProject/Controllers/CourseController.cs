using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController(AppGConetxt _context,IMapper _mapper ) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<CourseDTO>>> GetAllCourses()
        {
            var Coursers = await _context.Courses.ToListAsync();
            if (Coursers is null || Coursers.Count == 0)
            {
                return NotFound();
            }
            var coursesDTO = _mapper.Map<List<Course>, List<CourseDTO>>(Coursers);
            return Ok(coursesDTO);

        }

        [HttpGet("id")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int id)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            var courseDTO = _mapper.Map<Course, CourseDTO>(course);

            return Ok(courseDTO);
        }

        [HttpPost]

        public async Task<ActionResult<CourseDTO>> CreateCourse( CourseUpdateDTO courseUpdateDTO)
        {
            if (courseUpdateDTO == null)
            {
                return BadRequest();
            }

            var course = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);

            await _context.Courses.AddAsync(course);

            int res = await _context.SaveChangesAsync();

            if (res > 0)
            {
                var courseDTo = _mapper.Map<Course, CourseDTO>(course);
                return Ok(courseDTo);
            }

            return StatusCode(500);



        }

        [HttpDelete]

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

        [HttpPut("id")]

        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id , CourseUpdateDTO courseUpdateDTO)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if(course is null)
            {
                return NotFound();
            }

            course.Title = courseUpdateDTO.Title;
            course.Description  = courseUpdateDTO.Description;
            course.Category = courseUpdateDTO.Category;
            course.Year = courseUpdateDTO.Year;
            course.Status = courseUpdateDTO.Status;

          

            await _context.SaveChangesAsync();

            var courseDTo = _mapper.Map<Course, CourseDTO>(course);

            return Ok(courseDTo);
        }
    }
}
