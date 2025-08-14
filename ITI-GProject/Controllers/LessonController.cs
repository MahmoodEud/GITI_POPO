using Microsoft.AspNetCore.Authorization;

namespace ITI_GProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController(AppGConetxt context, IMapper mapper) : ControllerBase
    {

        //[Authorize]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLessons([FromQuery] int? pageNumber, [FromQuery] int? pageSize)
        {
            var query = context.Lessons.AsQueryable();

            int totalItems = await query.CountAsync();

            // Default values
            int currentPage = pageNumber.GetValueOrDefault(1);
            int currentSize = pageSize.GetValueOrDefault(10);

            // Validation
            if (currentPage <= 0 || currentSize <= 0)
                return BadRequest("PageNumber and PageSize must be positive integers.");

            if (currentSize > 100)
                return BadRequest("PageSize cannot exceed 100 items.");

            // Pagination
            var pagedLessons = await query
                .Skip((currentPage - 1) * currentSize)
                .Take(currentSize)
                .ToListAsync();

            var mappedLessons = mapper.Map<List<LessonDTO>>(pagedLessons);

            var result = new
            {
                TotalItems = totalItems,
                PageNumber = currentPage,
                PageSize = currentSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)currentSize),
                Data = mappedLessons
            };

            return Ok(result);
        }


        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonDTO>> GetLessonById(int id)
        {
            Lesson? lesson = await context.Lessons.FirstOrDefaultAsync(e => e.Id == id);

            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }
            var lessonDTO1 = mapper.Map<Lesson, LessonDTO>(lesson);
            return Ok(lessonDTO1);
        }
        [HttpPut("{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLesson(int id,LessonUpdateDto lessonDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool exists = await context.Lessons.AnyAsync(l => l.Title == lessonDto.title && l.Id != id);
            if (exists)
                return BadRequest("لا يمكن استخدام هذا العنوان لأنه موجود بالفعل حاول تغييره.");
            var lesson = await context.Lessons.FindAsync(id);
            if (lesson == null)
            {
                return NotFound("Lesson not found.");
            }
            mapper.Map(lessonDto, lesson);
            await context.SaveChangesAsync();
            return Ok("Lesson Updated Successfully !!!");
        }
        [HttpPut("by-title/{title}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateLessonByTitle([FromRoute] string title, [FromBody] LessonUpdateDto lessonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var lesson = await context.Lessons.FirstOrDefaultAsync(e => e.Title.ToLower() == title.ToLower());
            if (lesson == null)
                return NotFound("Lesson not found.");

            bool exists = await context.Lessons.AnyAsync(l => l.Title == lessonDto.title && l.Id != lesson.Id);
            if (exists)
                return BadRequest("لا يمكن استخدام هذا العنوان لأنه موجود بالفعل حاول تغييره.");

            mapper.Map(lessonDto, lesson);
            await context.SaveChangesAsync();
            return Ok("Lesson Updated Successfully !!!");
        }

        [HttpPost("filter")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> FilterLessons([FromBody] LessonFilterDto filter)
        {
            var query = context.Lessons.AsQueryable();

            if (filter.CourseId.HasValue)
                query = query.Where(l => l.CourseId == filter.CourseId);

            if (!string.IsNullOrWhiteSpace(filter.Title))
                query = query.Where(l => l.Title.Contains(filter.Title));

            var lessons = await query.ToListAsync();

            if (lessons.Count == 0)
                return NotFound("No matching lessons found.");

            var mappedLessons = mapper.Map<List<LessonDTO>>(lessons);
            return Ok(mappedLessons);
        }


        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateLesson([FromBody] LessonUpdateDto lessonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            bool exists = await context.Lessons
                .AnyAsync(l => l.Title.ToLower() == lessonDto.title.ToLower());
            if (exists)
                return BadRequest("هذا العنوان مستخدم بالفعل حاول تغييره.");
            var lesson = mapper.Map<Lesson>(lessonDto);
            await context.Lessons.AddAsync(lesson);
            await context.SaveChangesAsync();

            return Ok(new { message = "Lesson created successfully!", id = lesson.Id });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await context.Lessons.FirstOrDefaultAsync(e => e.Id == id);
            if (lesson == null)
                return NotFound("Lesson not found.");

            context.Lessons.Remove(lesson);
            await context.SaveChangesAsync();
            return Ok(new { message = "Lesson deleted successfully." });
        }

    }
}
