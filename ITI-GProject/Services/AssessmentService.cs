using ITI_GProject.Data.Models;
using ITI_GProject.DTOs.AssessmentDTO;
using ITI_GProject.DTOs.ChoicesDTO;
using ITI_GProject.DTOs.QuestionsDTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ITI_GProject.Services
{
    public class AssessmentService:IAssessments
    {

        private readonly AppGConetxt _db;
        public AssessmentService(AppGConetxt db)
        {
            _db = db;
        }

        private AssDTO MapToDto(Assessments assessment)
        {
            int questionCount = assessment.Questions?.Count ?? 0;

            string lessonName = assessment.Lesson?.Title;

            return new AssDTO
            {
                Id = assessment.Id,
                Max_Attempts = assessment.Max_Attempts,
                Passing_Score = assessment.Passing_Score,
                Time_Limit = assessment.Time_Limit,
                Starting_At = assessment.Starting_At,
                LessonId = assessment.LessonId,
                QuestionCount = questionCount,
                LessonName = lessonName,
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
            var AllAssessments = await _db.Assessments
                .Include(l => l.Lesson)
                .Include(q => q.Questions)
                .ToListAsync();

            return AllAssessments.Select(a => MapToDto(a));
        }



        public async Task<bool> ExistedAssessment(int id)
        {
            return await _db.Assessments.AnyAsync(a => a.Id==id);
        }




        // 1. First ensure your DbContext has proper relationship configuration
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Assessments>()
        //        .HasMany(a => a.Questions)
        //        .WithOne(q => q.assessment)
        //        .HasForeignKey(q => q.QuizId)  // Match your foreign key property name
        //        .OnDelete(DeleteBehavior.Cascade);

        //    modelBuilder.Entity<Question>()
        //        .HasMany(q => q.choices)
        //        .WithOne(c => c.question)
        //        .HasForeignKey(c => c.QuestionId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //}

        // 2. Modified Create Method
        public async Task<AssDTO> CreateNewAssessment(CreateNewDTO CreateAssDTO, List<QuesDTO> questions = null)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // Create and save assessment first
                var assessment = new Assessments
                {
                    Max_Attempts = CreateAssDTO.Max_Attempts,
                    Passing_Score = CreateAssDTO.Passing_Score,
                    Time_Limit = CreateAssDTO.Time_Limit,
                    Starting_At = CreateAssDTO.Starting_At,
                    LessonId = CreateAssDTO.LessonId
                };

                _db.Assessments.Add(assessment);
                await _db.SaveChangesAsync(); // This generates the assessment ID

                // Process questions if provided
                if (questions != null)
                {
                    foreach (var ques in questions)
                    {
                        var question = new Question
                        {
                            QuizId = assessment.Id, // Set foreign key
                            Header = ques.Header,
                            Body = ques.Body,
                            correctAnswer = ques.correctAnswer,
                            choices = ques.Choices?.Select(c => new Choice
                            {
                                ChoiceText = c.ChoiceText,
                                IsCorrect = c.IsCorrect
                            }).ToList()
                        };

                        _db.Questions.Add(question);
                    }
                }

                // Single SaveChanges for all operations
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return await GetAssessmetById(assessment.Id);
            }
            catch (Exception)
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

        //public Task<IEnumerable<AssDTO>> GetAllAssessments()
        //{
        //    throw new NotImplementedException();
        //}

        public Task<AssDTO?> UpdateAssessment(int id, UpdateAssDTO updateAssDTO)
        {
            throw new NotImplementedException();
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
            return await _db.Assessments
                .Where(a => a.LessonId == lessId)
                .Include(a => a.Lesson)
                .Include(a => a.Questions)
                    .ThenInclude(q => q.choices)
                .Select(a => new AssDTO
                {
                    Id = a.Id,
                    Max_Attempts = a.Max_Attempts,
                    Passing_Score = a.Passing_Score,
                    Time_Limit = a.Time_Limit,
                    Starting_At = a.Starting_At,
                    LessonId = a.LessonId,
                    LessonName = a.Lesson.Title,
                    Questions = a.Questions.Select(q => new QuesDTO
                    {
                        Id = q.Id,
                        Header = q.Header,
                        Body = q.Body,
                        correctAnswer = q.correctAnswer,
                        Choices = q.choices.Select(c => new ChoiceDTO
                        {
                            Id = c.Id,
                            ChoiceText = c.ChoiceText,
                            IsCorrect = c.IsCorrect,
                            QuestionId = q.Id
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();
        }
    }

       
    
}
