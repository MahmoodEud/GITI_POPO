using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Services
{
    public class CourseState
    {
        private readonly AppGConetxt _context;

        public CourseState(AppGConetxt context)
        {
            _context = context;
        }
        public async Task<CourseStatsDTO> GetStatsAsync()
        {
            
            var q = _context.Courses.AsNoTracking();

            var total = await q.CountAsync();
            var available = await q.CountAsync(c => c.IsAvailable);
            var free = await q.CountAsync(c => c.Price == 0);

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



    }
}
