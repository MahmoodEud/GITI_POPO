namespace ITI_GProject.Services
{
    public class NotificationsService: INotificationsService
    {
        private readonly AppGConetxt _ctx;
        private readonly ILocalClock _clock;

        public NotificationsService(AppGConetxt ctx, ILocalClock clock)
        {
            _ctx = ctx;
            _clock = clock;
        }


        public async Task<NotificationDto> CreateAsync(
                   int studentId, string title, string body,
                   NotificationType type = NotificationType.General, string? actionUrl = null)
        {
            var n = new Notification
            {
                StudentId = studentId,
                Title = title,
                Body = body ?? "",
                CreatedAt = _clock.Now(),
                IsRead = false,
                Type = type,
                ActionUrl = actionUrl
            };

            _ctx.Notifications.Add(n);
            await _ctx.SaveChangesAsync();

            return new NotificationDto
            {
                Id = n.Id,
                StudentId = n.StudentId,
                Title = n.Title,
                Body = n.Body,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                Type = n.Type,
                ActionUrl = n.ActionUrl
            };
        }
        public async Task<int> CreateManyAsync(IEnumerable<int> studentIds, string title, string body,
    NotificationType type = NotificationType.General, string? actionUrl = null)
        {
            var now = _clock.Now();
            var ids = studentIds.Distinct().ToList();
            if (ids.Count == 0) return 0;

            var items = ids.Select(id => new Notification
            {
                StudentId = id,
                Title = title,
                Body = body ?? "",
                CreatedAt = now,
                IsRead = false,
                Type = type,
                ActionUrl = actionUrl
            }).ToList();

            _ctx.Notifications.AddRange(items);
            return await _ctx.SaveChangesAsync();
        }

        public async Task<List<int>> ResolveAudienceAsync(int? year, int? courseId)
        {
            IQueryable<Student> q = _ctx.Students.AsNoTracking();

            if (year.HasValue)
            {
                if (!Enum.IsDefined(typeof(StudentYear), year.Value))
                    return new List<int>();

                var yearEnum = (StudentYear)year.Value;
                q = q.Where(s => s.Year == yearEnum);
            }

            if (courseId.HasValue)
            {
                q =
                    from s in q
                    join sc in _ctx.StudentCourses.AsNoTracking()
                        on s.Id equals sc.Student_Id
                    where sc.Course_Id == courseId.Value
                    select s;
            }

            return await q.Select(s => s.Id).Distinct().ToListAsync();
        }



        public async Task<IReadOnlyList<NotificationDto>> GetByStudentAsync(int studentId, int? take = null)
        {
            IQueryable<Notification> q = _ctx.Notifications
                .AsNoTracking()
                .Where(x => x.StudentId == studentId)
                .OrderByDescending(x => x.CreatedAt);

            if (take.HasValue)
                q = q.Take(take.Value);

            return await q.Select(x => new NotificationDto
            {
                Id = x.Id,
                StudentId = x.StudentId,
                Title = x.Title,
                Body = x.Body,
                CreatedAt = x.CreatedAt,
                IsRead = x.IsRead,
                Type = x.Type,              
                ActionUrl = x.ActionUrl     
            }).ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int studentId)
        {
            return await _ctx.Notifications.CountAsync(n => n.StudentId == studentId && !n.IsRead);
        }
        public async Task<bool> MarkAsReadAsync(int notificationId, int studentId)
        {
            var n = await _ctx.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.StudentId == studentId);
            if (n is null) return false;
            if (!n.IsRead)
            {
                n.IsRead = true;
                await _ctx.SaveChangesAsync();
            }
            return true;
        }
        public async Task<int> MarkAllAsReadAsync(int studentId)
        {
            var list = await _ctx.Notifications.Where(x => x.StudentId == studentId && !x.IsRead).ToListAsync();
            foreach (var n in list) n.IsRead = true;
            await _ctx.SaveChangesAsync();
            return list.Count;
        }

    }

}
