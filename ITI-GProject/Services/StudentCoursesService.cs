
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Services
{
    public class StudentCoursesService : IStudentCoursesService
    {
        private readonly AppGConetxt ctx;
        private readonly IMapper mapper;

        public StudentCoursesService(AppGConetxt ctx, IMapper mapper)
        {
            this.ctx = ctx;
            this.mapper = mapper;
        }


        public async Task<EnrollResult> EnrollAsync(int studentId, int courseId, bool allowPaid = false)
        {
            var course = await ctx.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course is null) return EnrollResult.CourseNotFound;

            var exists = await ctx.StudentCourses
                .AnyAsync(sc => sc.Student_Id == studentId && sc.Course_Id == courseId);
            if (exists) return EnrollResult.AlreadyEnrolled;

            var isPaid = course.Price > 0m;
            if (isPaid && !allowPaid) return EnrollResult.PaidNotAllowed;

            ctx.StudentCourses.Add(new StudentCourse { Student_Id = studentId, Course_Id = courseId });
            await ctx.SaveChangesAsync();
            return EnrollResult.Success;
        }

        public async Task<bool> UnenrollAsync(int studentId, int courseId)
        {
            var row = await ctx.StudentCourses
                .FirstOrDefaultAsync(x => x.Student_Id == studentId && x.Course_Id == courseId);
            if (row is null) return false;
            ctx.StudentCourses.Remove(row);
            return await ctx.SaveChangesAsync() > 0;
        }
        public async Task<IReadOnlyList<CourseDTO>> GetMyCoursesAsync(int studentId)
        {
            var q = from sc in ctx.StudentCourses.AsNoTracking()
                    join c in ctx.Courses.AsNoTracking() on sc.Course_Id equals c.Id
                    where sc.Student_Id == studentId
                    orderby c.UpdatedAt descending
                    select c;

            var list = await q.ProjectTo<CourseDTO>(mapper.ConfigurationProvider).ToListAsync();
            return list;
        }
        public async Task<bool> CanAccessContentAsync(int courseId, int? studentId, ClaimsPrincipal user)
        {
            if (user.IsInRole("Admin") || user.IsInRole("Assistant")) return true;

            if (user.IsInRole("Admin") || user.IsInRole("Assistant")) return true;

            var course = await ctx.Courses
                .AsNoTracking()
                .Select(c => new { c.Id, c.Price })
                .FirstOrDefaultAsync(c => c.Id == courseId);
            if (course is null) return false;

            if (course.Price == 0m) return true;

            if (studentId is null) return false;

            return await ctx.StudentCourses
                .AnyAsync(sc => sc.Student_Id == studentId.Value && sc.Course_Id == courseId);
        }


    }
}
