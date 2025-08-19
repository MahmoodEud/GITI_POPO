
namespace ITI_GProject.Services
{
    public class StudentResponseService : IStudentResponseService
    {
        private readonly AppGConetxt _ctx;

        public StudentResponseService(AppGConetxt ctx)
        {
            _ctx = ctx;
        }

        public async Task<StudentResponseDTO?> AddResponseAsync(int attemptId, StudentResponseCreateDTO responseDto)
        {
            var attempt = await _ctx.StudentAttempts
                .Include(a => a.Assessment)
                .ThenInclude(a => a.Questions)
                .FirstOrDefaultAsync(a => a.Id == attemptId);
            if (attempt == null) return null;

            var question = await _ctx.Questions
                .Include(q => q.choices)
                .FirstOrDefaultAsync(q => q.Id == responseDto.QuestionId && q.QuizId == attempt.AssessmentId);
            if (question == null) return null;

            var choice = await _ctx.Choices
                .FirstOrDefaultAsync(c => c.Id == responseDto.ChoiceId && c.QuestionId == question.Id);
            if (choice == null) return null;

            var exists = await _ctx.StudentResponses
                .AnyAsync(x => x.AttemptId == attemptId && x.QuestionId == responseDto.QuestionId);
            if (exists) throw new System.Exception("This question is already answered in this attempt.");

            var studentResponse = new StudentResponse
            {
                AttemptId = attemptId,
                QuestionId = responseDto.QuestionId,
                ChoiceId = responseDto.ChoiceId,
                IsCorrect = choice.IsCorrect
            };

            _ctx.StudentResponses.Add(studentResponse);
            await _ctx.SaveChangesAsync();

            return new StudentResponseDTO
            {
                Id = studentResponse.Id,
                AttemptId = studentResponse.AttemptId,
                QuestionId = studentResponse.QuestionId,
                ChoiceId = studentResponse.ChoiceId,
                IsCorrect = studentResponse.IsCorrect
            };
        }

        public async Task<IEnumerable<StudentResponseDTO>> GetResponsesByAttemptAsync(int attemptId)
        {
            return await _ctx.StudentResponses
                .AsNoTracking()
                .Where(r => r.AttemptId == attemptId)
                .Select(r => new StudentResponseDTO
                {
                    Id = r.Id,
                    AttemptId = r.AttemptId,
                    QuestionId = r.QuestionId,
                    ChoiceId = r.ChoiceId,
                    IsCorrect = r.IsCorrect
                })
                .ToListAsync();
        }

        public async Task<StudentResponseDTO?> GetResponseAsync(int id)
        {
            var response = await _ctx.StudentResponses.FindAsync(id);
            if (response == null) return null;

            return new StudentResponseDTO
            {
                Id = response.Id,
                AttemptId = response.AttemptId,
                QuestionId = response.QuestionId,
                ChoiceId = response.ChoiceId,
                IsCorrect = response.IsCorrect
            };
        }

        public async Task<bool> DeleteResponseAsync(int id)
        {
            var response = await _ctx.StudentResponses.FindAsync(id);
            if (response == null) return false;

            _ctx.StudentResponses.Remove(response);
            await _ctx.SaveChangesAsync();
            return true;
        }
    }
}
