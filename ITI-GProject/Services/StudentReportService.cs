namespace ITI_GProject.Services
{
    public class StudentReportService : IStudentReportService
    {
        private readonly AppGConetxt _ctx;
        public StudentReportService(AppGConetxt ctx) => _ctx = ctx;

        public async Task<StudentReportDto?> GetReportByAttemptIdAsync(int attemptId, int studentId)
        {
            var attempt = await _ctx.StudentAttempts
                .Include(a => a.Assessment)
                    .ThenInclude(a => a.Lesson)                    
                .Include(a => a.Assessment)
                    .ThenInclude(a => a.Questions)
                        .ThenInclude(q => q.choices)                
                .Include(a => a.StudentResponses)
                    .ThenInclude(sr => sr.Question)                 
                        .ThenInclude(q => q.choices)             
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == attemptId && a.StudentId == studentId);

            if (attempt == null || attempt.SubmittedAt == null)
                return null;

            var total = attempt.Assessment.Questions.Count;
            var passed = attempt.Score.HasValue && attempt.Score.Value >= attempt.Assessment.Passing_Score;

            var questions = attempt.StudentResponses.Select(r => new QuestionReportDto
            {
                QuestionId = r.QuestionId,
                QuestionText = r.Question?.Header ?? "",
                StudentAnswer = r.Question?.choices.FirstOrDefault(c => c.Id == r.ChoiceId)?.ChoiceText ?? "",
                CorrectAnswer = r.Question?.choices.FirstOrDefault(c => c.IsCorrect)?.ChoiceText ?? "",
                IsCorrect = r.IsCorrect
            }).ToList();

            return new StudentReportDto
            {
                AttemptId = attempt.Id,
                AssessmentId = attempt.AssessmentId,
                AssessmentName = attempt.Assessment.Lesson?.Title ?? $"Assessment #{attempt.AssessmentId}",
                Score = attempt.Score ?? 0,          
                TotalQuestions = total,
                Percentage = attempt.Score ?? 0,     
                IsPassed = passed,
                Questions = questions
            };
        }

        public async Task<IEnumerable<StudentAttemptSummaryDto>> GetAllReportsAsync(int studentId)
        {
            var attempts = await _ctx.StudentAttempts
                .Where(a => a.StudentId == studentId && a.SubmittedAt != null)
                .Include(a => a.Assessment)
                    .ThenInclude(x => x.Lesson)
                .Include(a => a.Assessment)
                    .ThenInclude(x => x.Questions)
                .AsNoTracking()
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            return attempts.Select(a =>
            {
                var total = a.Assessment?.Questions?.Count ?? 0;
                var score = a.Score ?? 0; // 0..100
                var passed = a.Assessment != null && score >= a.Assessment.Passing_Score;

                return new StudentAttemptSummaryDto
                {
                    AttemptId = a.Id,
                    AssessmentId = a.AssessmentId,
                    AssessmentName = a.Assessment?.Lesson?.Title ?? $"Assessment #{a.AssessmentId}",
                    Score = score,
                    TotalQuestions = total,
                    Percentage = score, // نفس الـ Score
                    IsPassed = passed,
                    StartedAt = a.StartedAt,
                    SubmittedAt = a.SubmittedAt
                };
            }).ToList();
        }

        public async Task<StudentReportsOverviewDto> GetOverviewAsync(int studentId, int recentCount = 10)
        {
            var attempts = await _ctx.StudentAttempts
                .Where(a => a.StudentId == studentId && a.SubmittedAt != null)
                .Include(a => a.Assessment)
                .AsNoTracking()
                .OrderBy(a => a.SubmittedAt)
                .ToListAsync();

            var dto = new StudentReportsOverviewDto
            {
                TotalAttempts = attempts.Count,
                DistinctAssessments = attempts.Select(a => a.AssessmentId).Distinct().Count()
            };

            var scored = attempts.Where(a => a.Score.HasValue).ToList();
            dto.AverageScore = scored.Any() ? Math.Round(scored.Average(a => a.Score!.Value), 2) : 0;

            var passCount = scored.Count(a => a.Assessment != null && a.Score!.Value >= a.Assessment.Passing_Score);
            dto.PassRate = scored.Any() ? Math.Round(passCount * 100.0 / scored.Count, 2) : 0;

            var attemptIds = attempts.Select(a => a.Id).ToList();
            if (attemptIds.Count > 0)
            {
                var agg = await _ctx.StudentResponses
                    .Where(sr => attemptIds.Contains(sr.AttemptId))
                    .GroupBy(_ => 1)
                    .Select(g => new
                    {
                        Correct = g.Count(x => x.IsCorrect),
                        Wrong = g.Count(x => !x.IsCorrect)
                    })
                    .FirstOrDefaultAsync();

                dto.TotalCorrect = agg?.Correct ?? 0;
                dto.TotalWrong = agg?.Wrong ?? 0;
            }

            if (scored.Count >= 2)
                dto.ImprovementPoints = (scored.Last().Score ?? 0) - (scored.First().Score ?? 0);

            var recent = attempts
                .Where(a => a.Score.HasValue)
                .OrderByDescending(a => a.SubmittedAt)
                .Take(recentCount)
                .ToList();
            recent.Reverse();

            dto.RecentScores = recent.Select(a => new RecentScorePointDto
            {
                Date = a.SubmittedAt ?? a.StartedAt,
                Score = a.Score ?? 0
            }).ToList();

            return dto;
        }
    }
}
