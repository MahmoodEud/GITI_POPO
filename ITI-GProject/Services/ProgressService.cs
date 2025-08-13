
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITI_GProject.Services
{
    public class ProgressService : IProgressService
    {
        private readonly AppGConetxt ctx;
        public ProgressService(AppGConetxt ctx) => this.ctx = ctx;


        public async Task<LessonProgressDTO> UpsertLessonAsync(int studentId, int lessonId, LessonProgressUpsertDTO dto)
        {
            var p = await ctx.StudentLessonProgresses
          .FirstOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == lessonId);
            if (p == null)
            {
                p = new StudentLessonProgress { StudentId = studentId, LessonId = lessonId };
                ctx.StudentLessonProgresses.Add(p);
            }
            p.WatchedSeconds = Math.Max(0, dto.WatchedSeconds);
            if (dto.DurationSeconds.HasValue)
                p.DurationSeconds = Math.Max(p.DurationSeconds, dto.DurationSeconds.Value);


            if (dto.IsCompleted.HasValue)
                p.IsCompleted = dto.IsCompleted.Value;

            if (!p.IsCompleted && p.DurationSeconds > 0 && p.WatchedSeconds >= 0.9 * p.DurationSeconds)
                p.IsCompleted = true;
            p.UpdatedAt = DateTime.UtcNow;
            await ctx.SaveChangesAsync();
            return new LessonProgressDTO
            {
                LessonId = lessonId,
                WatchedSeconds = p.WatchedSeconds,
                DurationSeconds = p.DurationSeconds,
                IsCompleted = p.IsCompleted,
                UpdatedAt = p.UpdatedAt
            };
        }

        public async Task<CourseProgressDTO?> GetCourseAsync(int studentId, int courseId)
        {
            var lessonIds = await ctx.Lessons
                .Where(l => l.CourseId == courseId)
                .Select(l => l.Id)
                .ToListAsync();

            if (lessonIds.Count == 0) return null;

            var map = await ctx.StudentLessonProgresses.AsNoTracking()
                .Where(p => p.StudentId == studentId && lessonIds.Contains(p.LessonId))
                .ToDictionaryAsync(p => p.LessonId, p => p);

            var lessons = lessonIds.Select(id =>
            {
                map.TryGetValue(id, out var p);
                return new LessonProgressDTO
                {
                    LessonId = id,
                    WatchedSeconds = p?.WatchedSeconds ?? 0,
                    DurationSeconds = p?.DurationSeconds ?? 0,
                    IsCompleted = p?.IsCompleted ?? false,
                    UpdatedAt = p?.UpdatedAt ?? DateTime.MinValue
                };
            }).ToList();

            var completed = lessons.Count(x => x.IsCompleted);

            return new CourseProgressDTO
            {
                CourseId = courseId,
                TotalLessons = lessons.Count,
                CompletedLessons = completed,
                Percent = lessons.Count == 0 ? 0 : Math.Round(100.0 * completed / lessons.Count, 2),
                Lessons = lessons
            };
        }

        public async Task<LessonProgressDTO?> GetLessonAsync(int studentId, int lessonId)
        {
            var p = await ctx.StudentLessonProgresses.AsNoTracking()
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.LessonId == lessonId);
            if (p == null) return null;

            return new LessonProgressDTO
            {
                LessonId = lessonId,
                WatchedSeconds = p.WatchedSeconds,
                DurationSeconds = p.DurationSeconds,
                IsCompleted = p.IsCompleted,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}