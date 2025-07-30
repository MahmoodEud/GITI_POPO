using ITI_GProject.DTOs.ChoicesDTO;

namespace ITI_GProject.Services
{
    public class ChoiceService:IChoice
    {

        private readonly IMapper _mapper;
        private readonly AppGConetxt _db;

        public ChoiceService(IMapper mapper, AppGConetxt db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<ChoiceDTO> CreateChoice(CreateDTO careteDto)
        {

            var choice = _mapper.Map<Choice>(careteDto);
            await _db.Choices.AddAsync(choice);
            await _db.SaveChangesAsync();

            return _mapper.Map<ChoiceDTO>(choice);
        }

        public async Task DeleteChoice(int id)
        {

            var DeletedChoice = await _db.Choices.FindAsync(id);
            if(DeletedChoice == null)
            {
                throw new KeyNotFoundException($"Sorry ! Choice with id : {id} was Not Found ");
            }

             _db.Choices.Remove(DeletedChoice);
            await _db.SaveChangesAsync();


        }

        public async Task<ChoiceDTO> GetChoiceById(int id)
        {

            var choice = await _db.Choices.FindAsync(id);
            if(choice == null)
            {
                throw new KeyNotFoundException($"Sorry No Choice with Id : {id}");

            }

            return _mapper.Map<ChoiceDTO>(choice);
        }

        public async Task<IEnumerable<ChoiceDTO>> GetChoicesByQuestion(int questionId)
        {

            var choices = await _db.Choices
                .Where(q => q.QuestionId == questionId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ChoiceDTO>>(choices);
        }

        public async Task<ChoiceDTO> UpdateChoice(int id, UpdateDTO updateDto)
        {

            var choice = await _db.Choices.FindAsync(id);
            if (choice == null)
            {
                throw new KeyNotFoundException($"Sorry No Choice With Id : {id}");
            }

            _mapper.Map(updateDto, choice);
            _db.Choices.Update(choice);
            await _db.SaveChangesAsync();

            return _mapper.Map<ChoiceDTO>(choice);
        }
    }
}
