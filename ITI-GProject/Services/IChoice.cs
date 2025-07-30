using ITI_GProject.DTOs.ChoicesDTO;

namespace ITI_GProject.Services
{
    public interface IChoice
    {
        Task<IEnumerable<ChoiceDTO>> GetChoicesByQuestion(int questionId);
        Task<ChoiceDTO> CreateChoice(CreateDTO careteDto);
        Task<ChoiceDTO> GetChoiceById(int id);
        Task<ChoiceDTO> UpdateChoice(int id, UpdateDTO updateDto);
        Task DeleteChoice(int id);



    }
}
