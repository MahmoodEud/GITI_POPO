
using AutoMapper.QueryableExtensions;
using ITI_GProject.Services.Attachment;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging.Abstractions;

namespace ITI_GProject.Services
{
    public class CourseServices(AppGConetxt _context, IMapper _mapper, IAttachment attachment) : ICourseService
    {
        public async Task<PagedResult<CourseDTO>> GetAllCoursesAsync(
            string? search = null,
            CourseStatus? status = null,
            string? category = null,
            int page = 1,
            int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 10, 100);

            var query = _context.Courses.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(c => c.Title.ToLower().Contains(s));
            }

            if (status.HasValue) query = query.Where(c => c.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(category)) query = query.Where(c => c.Category == category);

            query = query.OrderByDescending(c => c.UpdatedAt);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<CourseDTO>(_mapper.ConfigurationProvider) // أو Map بعد ToListAsync
            .ToListAsync();


            return new PagedResult<CourseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages
            };




        }
        public async Task<CourseDTO?> GetCourseByIdAsync(int courseId)
        {
            return await _context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .ProjectTo<CourseDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }


        public async Task<CourseDTO?> CreateCourseAsync(CourseUpdateDTO courseUpdateDTO)
        {
            if (courseUpdateDTO == null)
                return null!;
            if (courseUpdateDTO.Price < 0) return null;

            //var course = _mapper.Map<CourseUpdateDTO, Course>(courseUpdateDTO);
            var course = _mapper.Map<Course>(courseUpdateDTO);
            course.Status = CourseStatus.Draft;
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;
            course.PicturalUrl = "default.jpg";
            if (courseUpdateDTO.PicturalUrl is not null && courseUpdateDTO.PicturalUrl.Length > 0)
            {
                var uploadedPath = await attachment.UploadAsync(courseUpdateDTO.PicturalUrl, "courses");
                if (!string.IsNullOrWhiteSpace(uploadedPath))
                    course.PicturalUrl = uploadedPath ?? "";
            }
            else
            {
                course.PicturalUrl = "default.jpg";
            }

            await _context.Courses.AddAsync(course);
            var res = await _context.SaveChangesAsync();

            if (res > 0)
            {
                var courseDto = _mapper.Map<Course, CourseDTO>(course);
                return courseDto;
            }
            else
            {
                return null!;
            }

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

        public async Task<CourseDTO?> UpdateCourseAsync(int id, CourseUpdateDTO dto)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
            if (course is null || dto is null) return null;

            if (dto.PicturalUrl is not null && dto.PicturalUrl.Length > 0)
            {
                if (!string.IsNullOrWhiteSpace(course.PicturalUrl) && !course.PicturalUrl.Contains("default.jpg"))
                    await attachment.DeleteAsync(course.PicturalUrl);

                var uploadedPath = await attachment.UploadAsync(dto.PicturalUrl, "courses");
                if (!string.IsNullOrWhiteSpace(uploadedPath))
                    course.PicturalUrl = uploadedPath;
            }

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Category = dto.Category;
            course.Year = dto.Year;
            course.Price = dto.Price;
            course.IsAvailable = dto.IsAvailable;   
            course.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return _mapper.Map<CourseDTO>(course);
        }

        public async Task<CourseStatsDTO> GetStatsAsync()
        {
            var q = _context.Courses.AsNoTracking();

            var total = await q.CountAsync();
            var available = await q.CountAsync(c => c.IsAvailable);
            var free = await q.CountAsync(c => c.Price == 0m);

            var byYear = await q
                .GroupBy(c => c.Year)
                .Select(g => new { Year = g.Key.ToString(), Count = g.Count() })
                .ToDictionaryAsync(x => x.Year, x => x.Count);

            return new CourseStatsDTO
            {
                Total = total,
                Available = available,
                Unavailable = total - available,
                Free = free,
                Paid = total - free,
                ByYear = byYear
            };
        }

        public async Task<CourseContentDTO?> GetCourseContentAsync(int courseId)
        {
            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course is null) return null;

            var lessons = await _context.Lessons
                .AsNoTracking()
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Id) 
                .Select(l => new LessonDTO
                {
                    Id = l.Id,
                    Title = l.Title,
                    VideoUrl = l.VideoUrl,
                    PreviewVideoUrl = l.PreviewVideoUrl,
                    PdfUrl = l.PdfUrl
                })
                .ToListAsync();

            return new CourseContentDTO
            {
                CourseId = course.Id,
                Title = course.Title,
                Lessons = lessons
            };
        }


    }
}