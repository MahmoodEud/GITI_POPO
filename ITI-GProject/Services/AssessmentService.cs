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
                MaxAttempts = assessment.Max_Attempts,
                PassingScore = assessment.Passing_Score,
                TimeLimit = assessment.Time_Limit,
                StartingAt = assessment.Starting_At,
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
                    Max_Attempts = dto.MaxAttempts,
                    Passing_Score = dto.PassingScore,
                    Time_Limit = dto.TimeLimit,
                    Starting_At = dto.StartingAt,
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
   
        public async Task<DeleteAssessmentResult> DeleteAssessment(int id)
        {
            var hasAttempts = await _db.StudentAttempts.AnyAsync(a => a.AssessmentId == id);
            if (hasAttempts) return DeleteAssessmentResult.HasAttempts;

            var assessment = await _db.Assessments
                .Include(a => a.Questions)
                    .ThenInclude(q => q.choices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null) return DeleteAssessmentResult.NotFound;

            _db.Assessments.Remove(assessment);
            await _db.SaveChangesAsync();
            return DeleteAssessmentResult.Deleted;
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
                MaxAttempts = a.Max_Attempts,
                PassingScore = a.Passing_Score,
                TimeLimit = a.Time_Limit,
                StartingAt = a.Starting_At,
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
        public async Task<IEnumerable<StudentAttemptDTO>> GetAttemptsByAssessmentAsync(int assessmentId)
        {
            return await _db.StudentAttempts
                .Where(a => a.AssessmentId == assessmentId)
                .Include(a => a.Student)
                .Select(a => new StudentAttemptDTO
                {
                    Id = a.Id,
                    StudentName = a.Student.User.Name,   
                    StudentId = a.StudentId,
                    AssessmentId = a.AssessmentId,
                    AttemptNumber = a.AttemptsNumber,
                    StartedAt = a.StartedAt,
                    SubmittedAt = a.SubmittedAt,
                    Score = a.Score,
                    IsGraded = a.Score.HasValue
                })
                .ToListAsync();
        }

        public async Task<AssDTO?> UpdateAssessment(int id, UpdateAssDTO dto)
        {
            var assessment = await _db.Assessments
                .Include(a => a.Questions)
                    .ThenInclude(q => q.choices)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assessment == null)
                return null;

            assessment.LessonId = dto.LessonId;
            assessment.Max_Attempts = dto.MaxAttempts;
            assessment.Passing_Score = dto.PassingScore;
            assessment.Time_Limit = dto.TimeLimit;
            assessment.Starting_At = dto.StartingAt;

            var dtoQIds = dto.Questions.Where(q => q.Id > 0).Select(q => q.Id).ToHashSet();

            var questionsToRemove = assessment.Questions
                .Where(q => !dtoQIds.Contains(q.Id))
                .ToList();
            _db.Questions.RemoveRange(questionsToRemove);

            foreach (var qdto in dto.Questions)
            {
                var qentity = (qdto.Id > 0)
                    ? assessment.Questions.FirstOrDefault(q => q.Id == qdto.Id)
                    : null;

                if (qentity == null)
                {
                    qentity = new Question
                    {
                        Header = qdto.Header ?? string.Empty,
                        Body = qdto.Body ?? string.Empty
                    };
                    assessment.Questions.Add(qentity);
                }
                else
                {
                    qentity.Header = qdto.Header ?? string.Empty;
                    qentity.Body = qdto.Body ?? string.Empty;
                }

                var dtoCIds = qdto.Choices.Where(c => c.Id > 0).Select(c => c.Id).ToHashSet();

                var choicesToRemove = qentity.choices
                    .Where(c => !dtoCIds.Contains(c.Id))
                    .ToList();
                _db.Choices.RemoveRange(choicesToRemove);

                foreach (var cdto in qdto.Choices)
                {
                    var centity = (cdto.Id > 0)
                        ? qentity.choices.FirstOrDefault(c => c.Id == cdto.Id)
                        : null;

                    if (centity == null)
                    {
                        centity = new Choice
                        {
                            ChoiceText = cdto.ChoiceText ?? string.Empty,
                            IsCorrect = cdto.IsCorrect
                        };
                        qentity.choices.Add(centity);
                    }
                    else
                    {
                        centity.ChoiceText = cdto.ChoiceText ?? string.Empty;
                        centity.IsCorrect = cdto.IsCorrect;
                    }
                }
            }

            await _db.SaveChangesAsync();
            return await GetAssessmetById(assessment.Id);
        }

        public async Task<bool> DeleteAttemptsByAssessmentId(int assessmentId)
        {
            var attempts = await _db.StudentAttempts
                .Where(sa => sa.AssessmentId == assessmentId)
                .ToListAsync();

            if (!attempts.Any())
                return false;

            _db.StudentAttempts.RemoveRange(attempts);
            await _db.SaveChangesAsync();
            return true;
        }




    }
}
