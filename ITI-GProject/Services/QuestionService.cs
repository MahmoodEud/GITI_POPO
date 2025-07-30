using ITI_GProject.DTOs.ChoicesDTO;
using ITI_GProject.DTOs.QuestionsDTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ITI_GProject.Services
{
    public class QuestionService : IQuestion
    {

        private readonly IMapper _mapper;
        private readonly AppGConetxt _db;


        public QuestionService(IMapper mapper, AppGConetxt db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<QuesDTO> CreateNewQuestion(QuesDTO dto)
        {
            var question = _mapper.Map<Question>(dto);
            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return _mapper.Map<QuesDTO>(question);


        }

        public async Task<bool> DeletQuestion(int id)
        {
            var question = await _db.Questions.FindAsync(id);
            if (question == null)
            {
                return false;

            }

            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();
            return true;


        }

        public async Task<QuesDTO> GetQuestionById(int id)
        {
            var question = await _db.Questions
                .Include(c => c.choices)
                .FirstOrDefaultAsync(c => c.Id == id);
            return _mapper.Map<QuesDTO>(question);

        }

        public async Task<IEnumerable<QuesDTO>> GetQuestionByQuizId(int quizId)
        {
            var questions = await _db.Questions
                .Include(c => c.choices)
                .Include(q => q.QuizId == quizId)
                .ToListAsync();
            return _mapper.Map<IEnumerable<QuesDTO>>(questions);

        }

        public async Task<QuesDTO> UpdateQuestion(int id, QuesDTO dto)
        {
            var existQuestion = await _db.Questions
                .Include(c => c.choices)
                .FirstOrDefaultAsync(q => q.Id == id);
            if (existQuestion == null)
            {
                throw new KeyNotFoundException($"No Question with id : {id}");
            }
            existQuestion.Header = dto.Header;
            existQuestion.Body = dto.Body;
            existQuestion.QuizId = dto.QuizId;
            existQuestion.correctAnswer = dto.correctAnswer;

            UpdateQuestionChoices(existQuestion, dto.Choices);
            await _db.SaveChangesAsync();
            return _mapper.Map<QuesDTO>(existQuestion);
        }



       private void UpdateQuestionChoices(Question question, ICollection<ChoiceDTO> choiceDtos)
        {
            // Remove choices not in the DTO
            var choicesToRemove = question.choices
                .Where(c => !choiceDtos.Any(dto => dto.Id == c.Id))
                .ToList();

            foreach (var choice in choicesToRemove)
            {
                _db.Choices.Remove(choice);
            }

            foreach (var choiceDto in choiceDtos)
            {
                var existingChoice = question.choices.FirstOrDefault(c => c.Id == choiceDto.Id);
                if (existingChoice != null)
                {
                    _mapper.Map(choiceDto, existingChoice);
                }
                else
                {
                    var newChoice = _mapper.Map<Choice>(choiceDto);
                    newChoice.QuestionId = question.Id;
                    question.choices.Add(newChoice);
                }
            }
        }



     

    }
}
