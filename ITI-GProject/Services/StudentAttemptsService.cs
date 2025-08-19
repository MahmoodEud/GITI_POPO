namespace ITI_GProject.Services
{
    public class StudentAttemptsService : IStudentAttemptsService
    {
        private readonly AppGConetxt _ctx;

        public StudentAttemptsService(AppGConetxt ctx)
        {
            _ctx = ctx;
        }

        public async Task<StudentAttemptDTO?> StartAttemptAsync(int studentId, int assessmentId)
        {
            var assessment = await _ctx.Assessments
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == assessmentId);

            if (assessment == null) return null;

            int? maxAttempts = assessment.Max_Attempts;
            int? timeLimit = assessment.Time_Limit;   // null => كويز مفتوح

            // لو في محاولة مفتوحة، رجّعها
            var openAttempt = await _ctx.StudentAttempts
                .Where(a => a.StudentId == studentId
                            && a.AssessmentId == assessmentId
                            && a.SubmittedAt == null)
                .OrderByDescending(a => a.StartedAt)
                .FirstOrDefaultAsync();

            if (openAttempt != null)
                return ToDto(openAttempt, timeLimit);

            // تحقق من الحد الأقصى للمحاولات (إن وُجد)
            if (maxAttempts.HasValue)
            {
                var count = await _ctx.StudentAttempts
                    .CountAsync(a => a.StudentId == studentId && a.AssessmentId == assessmentId);

                if (count >= maxAttempts.Value)
                    return null;
            }

            // احسب رقم المحاولة التالية
            int nextNumber = (await _ctx.StudentAttempts
                .Where(a => a.StudentId == studentId && a.AssessmentId == assessmentId)
                .Select(a => (int?)a.AttemptsNumber)
                .MaxAsync()) ?? 0;

            var attempt = new StudentAttempts
            {
                StudentId = studentId,
                AssessmentId = assessmentId,
                AttemptsNumber = nextNumber + 1,
                StartedAt = DateTime.UtcNow,
                SubmittedAt = null,
                Score = null
            };

            _ctx.StudentAttempts.Add(attempt);
            await _ctx.SaveChangesAsync();

            return ToDto(attempt, timeLimit);
        }

        private static StudentAttemptDTO ToDto(StudentAttempts a, int? timeLimit)
        {
            return new StudentAttemptDTO
            {
                Id = a.Id,
                StudentId = a.StudentId,
                AssessmentId = a.AssessmentId,
                AttemptNumber = a.AttemptsNumber,
                StartedAt = a.StartedAt,
                SubmittedAt = a.SubmittedAt,
                TimeLimitMinutes = timeLimit,         
                Score = a.Score,
                IsGraded = a.Score != null
            };
        }



        public async Task<StudentAttemptDTO?> SubmitAttemptAsync(int attemptId, List<StudentResponseDTO> responses)
        {
            var attempt = await _ctx.StudentAttempts
                .Include(a => a.Assessment)
                    .ThenInclude(ass => ass.Questions)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null) return null;

            // Upsert responses
            foreach (var r in responses)
            {
                var choice = await _ctx.Choices
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == r.ChoiceId);

                var isCorrect = choice?.IsCorrect ?? false;

                var existing = await _ctx.StudentResponses
                    .FirstOrDefaultAsync(x => x.AttemptId == attempt.Id && x.QuestionId == r.QuestionId);

                if (existing is null)
                {
                    _ctx.StudentResponses.Add(new StudentResponse
                    {
                        AttemptId = attempt.Id,
                        QuestionId = r.QuestionId,
                        ChoiceId = r.ChoiceId,
                        IsCorrect = isCorrect
                    });
                }
                else
                {
                    existing.ChoiceId = r.ChoiceId;
                    existing.IsCorrect = isCorrect;
                    _ctx.StudentResponses.Update(existing);
                }
            }

            // خلّي الإجابات تتخزن فعلياً
            await _ctx.SaveChangesAsync();

            attempt.SubmittedAt = DateTime.UtcNow;

            // احسب السكور في أي محاولة
            var totalQuestions = attempt.Assessment.Questions.Count;
            if (totalQuestions > 0)
            {
                var correct = await _ctx.StudentResponses
                    .Where(sr => sr.AttemptId == attempt.Id && sr.IsCorrect)
                    .CountAsync();

                attempt.Score = (int)Math.Round(100.0 * correct / totalQuestions);
            }

            await _ctx.SaveChangesAsync();

            return ToDto(attempt, attempt.Assessment.Time_Limit);
        }

        public async Task<IEnumerable<StudentAttemptDTO>> GetAttemptsByStudentAsync(int studentId)
        {
            return await _ctx.StudentAttempts
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Assessment)
                .OrderByDescending(a => a.StartedAt)
                .Select(a => new StudentAttemptDTO
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    AssessmentId = a.AssessmentId,
                    AttemptNumber = a.AttemptsNumber,
                    StartedAt = a.StartedAt,
                    SubmittedAt = a.SubmittedAt,
                    TimeLimitMinutes = a.Assessment.Time_Limit,
                    Score = a.Score,
                    IsGraded = a.Score != null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentAttemptDTO>> GetAttemptsByAssessmentAsync(int assessmentId)
        {
            return await _ctx.StudentAttempts
                .Where(a => a.AssessmentId == assessmentId)
                .Include(a => a.Assessment)
                .OrderByDescending(a => a.StartedAt)
                .Select(a => new StudentAttemptDTO
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    AssessmentId = a.AssessmentId,
                    AttemptNumber = a.AttemptsNumber,
                    StartedAt = a.StartedAt,
                    SubmittedAt = a.SubmittedAt,
                    TimeLimitMinutes = a.Assessment.Time_Limit,
                    Score = a.Score,
                    IsGraded = a.Score != null
                })
                .ToListAsync();
        }
    }
}
