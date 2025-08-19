using ITI_GProject.DTOs.ChoicesDTO;

namespace ITI_GProject.Services
{
    public class AssessmentService : IAssessments
    {
        private readonly AppGConetxt _db;
        public AssessmentService(AppGConetxt db)
        {
            _db = db;
        }

        private AssDTO MapToDto(Assessments assessment)
        {
            int questionCount = assessment.Questions?.Count ?? 0;
            string lessonName = assessment.Lesson?.Title ?? string.Empty;

            return new AssDTO
            {
                Id = assessment.Id,
                Max_Attempts = assessment.Max_Attempts,
                Passing_Score = assessment.Passing_Score,
                Time_Limit = assessment.Time_Limit,
                Starting_At = assessment.Starting_At,
                LessonId = assessment.LessonId,
                LessonName = lessonName,
                QuestionCount = questionCount,
                Questions = assessment.Questions?.Select(q => new QuesDTO
                {
                    Id = q.Id,
                    Header = q.Header,
                    Body = q.Body,
                    correctAnswer = q.correctAnswer,
                    Choices = q.choices?.Select(c => new ChoiceDTO
                    {
                        Id = c.Id,
                        ChoiceText = c.ChoiceText,
                        IsCorrect = c.IsCorrect,
                        QuestionId = q.Id
                    }).ToList() ?? new List<ChoiceDTO>()
                }).ToList() ?? new List<QuesDTO>()
            };
        }

        public async Task<IEnumerable<AssDTO>> GetAllAssessments()
        {
            var all = await _db.Assessments
                .Include(l => l.Lesson)
                .Include(q => q.Questions)
                    .ThenInclude(q => q.choices)
                .ToListAsync();

            return all.Select(a => MapToDto(a));
        }

        public async Task<bool> ExistedAssessment(int id)
        {
            return await _db.Assessments.AnyAsync(a => a.Id == id);
        }

        public async Task<AssDTO> CreateNewAssessment(CreateNewDTO dto)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                var assessment = new Assessments
                {
                    Max_Attempts = dto.Max_Attempts,
                    Passing_Score = dto.Passing_Score,
                    Time_Limit = dto.Time_Limit,
                    Starting_At = dto.Starting_At,
                    LessonId = dto.LessonId
                };

                _db.Assessments.Add(assessment);
                await _db.SaveChangesAsync();

                if (dto.Questions != null && dto.Questions.Count > 0)
                {
                    foreach (var q in dto.Questions)
                    {
                        var question = new Question
                        {
                            QuizId = assessment.Id,
                            Header = q.Header,
                            Body = q.Body,
                            correctAnswer = q.correctAnswer,
                            choices = q.Choices?.Select(c => new Choice
                            {
                                ChoiceText = c.ChoiceText,
                                IsCorrect = c.IsCorrect
                            }).ToList()
                        };

                        _db.Questions.Add(question);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetAssessmetById(assessment.Id) ?? MapToDto(assessment);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<AssDTO?> GetAssessmetById(int id)
        {
            var assess = await _db.Assessments
                .Include(l => l.Lesson)
                .Include(q => q.Questions)
                    .ThenInclude(c => c.choices)
                .FirstOrDefaultAsync(a => a.Id == id);

            return assess != null ? MapToDto(assess) : null;
        }

        public async Task<bool> DeleteAssessment(int id)
        {
            var assessment = await _db.Assessments
                .Include(a => a.Questions)
                    .ThenInclude(q => q.choices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null) return false;

            _db.Assessments.Remove(assessment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AssDTO>> GetAssessmentByLessonId(int lessId)
        {
            var list = await _db.Assessments
                .Where(a => a.LessonId == lessId)
                .Include(a => a.Lesson)
                .Include(a => a.Questions)
                    .ThenInclude(q => q.choices)
                .ToListAsync();

            return list.Select(a => new AssDTO
            {
                Id = a.Id,
                Max_Attempts = a.Max_Attempts,
                Passing_Score = a.Passing_Score,
                Time_Limit = a.Time_Limit,
                Starting_At = a.Starting_At,
                LessonId = a.LessonId,
                LessonName = a.Lesson?.Title ?? string.Empty,
                QuestionCount = a.Questions?.Count ?? 0,
                Questions = a.Questions?.Select(q => new QuesDTO
                {
                    Id = q.Id,
                    Header = q.Header,
                    Body = q.Body,
                    correctAnswer = q.correctAnswer,
                    Choices = q.choices?.Select(c => new ChoiceDTO
                    {
                        Id = c.Id,
                        ChoiceText = c.ChoiceText,
                        IsCorrect = c.IsCorrect,
                        QuestionId = q.Id
                    }).ToList() ?? new List<ChoiceDTO>()
                }).ToList() ?? new List<QuesDTO>()
            });
        }

        public Task<AssDTO?> UpdateAssessment(int id, UpdateAssDTO updateAssDTO)
        {
            throw new NotImplementedException();
        }
    }
}
