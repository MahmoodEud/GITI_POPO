using System;

namespace ITI_GProject.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppGConetxt _db;
        public ReportsService(AppGConetxt db) { _db = db; }

        public async Task<IEnumerable<AssessmentSummaryDto>> GetAssessmentsSummary()
        {
            var query = from a in _db.Assessments
                        join at in _db.StudentAttempts on a.Id equals at.AssessmentId into g
                        from atg in g.DefaultIfEmpty()
                        group atg by new { a.Id, a.Lesson.Title, a.Passing_Score } into grp
                        select new
                        {
                            grp.Key.Id,
                            Title = grp.Key.Title,
                            PassingScore = grp.Key.Passing_Score,
                            Attempts = grp.Where(x => x != null)
                        };

            var list = await query.AsNoTracking().ToListAsync();

            return list.Select(x =>
            {
                var attempts = x.Attempts.ToList();
                var scores = attempts.Where(t => t != null && t.Score.HasValue)
                                       .Select(t => t.Score!.Value)
                                       .OrderBy(v => v)
                                       .ToList();
                var avg = scores.Any() ? scores.Average() : 0;
                var median = scores.Count switch
                {
                    0 => 0,
                    var n when n % 2 == 1 => scores[n / 2],
                    var n => (scores[n / 2 - 1] + scores[n / 2]) / 2.0
                };
                var passes = scores.Count == 0 ? 0 : scores.Count(s => s >= x.PassingScore);
                var passRate = scores.Count == 0 ? 0 : (passes * 100.0 / scores.Count);
                var uniqueStudents = attempts.Where(t => t != null).Select(t => t!.StudentId).Distinct().Count();
                var lastAttempt = attempts.Where(t => t != null).Select(t => t!.SubmittedAt ?? t!.StartedAt).OrderByDescending(d => d).FirstOrDefault();

                return new AssessmentSummaryDto
                {
                    AssessmentId = x.Id,
                    AssessmentName = x.Title ?? $"Assessment #{x.Id}",
                    AttemptsCount = attempts.Count(t => t != null),
                    UniqueStudents = uniqueStudents,
                    AverageScore = Math.Round(avg, 2),
                    MedianScore = Math.Round(median, 2),
                    PassRate = Math.Round(passRate, 2),
                    LastAttemptAt = lastAttempt
                };
            }).OrderByDescending(r => r.LastAttemptAt).ToList();
        }

        public async Task<IEnumerable<QuestionDifficultyDto>> GetQuestionDifficulty(int assessmentId)
        {
            var questions = await _db.Questions
                .Where(q => q.QuizId == assessmentId)   
                .Select(q => new { q.Id, q.Header })
                .ToListAsync();


            var answersAgg = await _db.StudentResponses
                .Where(r => r.Attempt.AssessmentId == assessmentId)
                .GroupBy(r => r.QuestionId)
                .Select(g => new { QuestionId = g.Key, Total = g.Count(), Wrong = g.Count(x => !x.IsCorrect) })
                .ToListAsync();

            var map = answersAgg.ToDictionary(x => x.QuestionId, x => x);
            return questions.Select(q => {
                var a = map.ContainsKey(q.Id) ? map[q.Id] : null;
                var total = a?.Total ?? 0;
                var wrong = a?.Wrong ?? 0;
                var rate = total == 0 ? 0 : wrong * 100.0 / total;
                return new QuestionDifficultyDto
                {
                    QuestionId = q.Id,
                    Header = q.Header ?? $"Q#{q.Id}",
                    TotalAnswers = total,
                    WrongCount = wrong,
                    WrongRate = Math.Round(rate, 2)
                };
            })
            .OrderByDescending(x => x.WrongRate)
            .ToList();
        }

        public async Task<IEnumerable<StudentPerformanceDto>> GetStudentsPerformance(int? assessmentId = null)
        {
            var attempts = _db.StudentAttempts.AsQueryable();
            if (assessmentId.HasValue)
                attempts = attempts.Where(a => a.AssessmentId == assessmentId);

            var q = from a in attempts
                    join s in _db.Students on a.StudentId equals s.Id
                    select new { a.StudentId, s.User.Name, s.PhoneNumber, s.User.UserName, s.Year,s.ParentNumber, a.Score, a.Assessment.Passing_Score };

            var list = await q.AsNoTracking().ToListAsync();

            return list.GroupBy(x => new { x.StudentId, x.Name, x.PhoneNumber,x.UserName })
                .Select(g => {
                    var scores = g.Where(p => p.Score.HasValue).Select(p => p.Score!.Value).ToList();
                    var avg = scores.Any() ? scores.Average() : 0;
                    var passRate = scores.Count == 0 ? 0 : (scores.Count(s => s >= g.First().Passing_Score) * 100.0 / scores.Count);
                    return new StudentPerformanceDto
                    {
                        StudentId = g.Key.StudentId,
                        StudentName = g.Key.Name ?? $"Student #{g.Key.StudentId}",
                        phoneNumber = g.Key.PhoneNumber ?? "",
                        AttemptsCount = g.Count(),
                        AverageScore = Math.Round(avg, 2),
                        PassRate = Math.Round(passRate, 2)
                    };
                })
                .OrderByDescending(x => x.AverageScore)
                .ToList();
        }

        public async Task<IEnumerable<AttemptsTimeSeriesDto>> GetAttemptsTimeSeries(int? assessmentId = null, int days = 30)
        {
            var since = DateTime.UtcNow.Date.AddDays(-days);
            var attempts = _db.StudentAttempts.Where(a => (a.SubmittedAt ?? a.StartedAt) >= since);
            if (assessmentId.HasValue) attempts = attempts.Where(a => a.AssessmentId == assessmentId);

            var data = await attempts
                .GroupBy(a => (a.SubmittedAt ?? a.StartedAt).Date)
                .Select(g => new AttemptsTimeSeriesDto { Day = g.Key, Attempts = g.Count() })
                .OrderBy(x => x.Day)
                .ToListAsync();

            return data;
        }
    }
}
