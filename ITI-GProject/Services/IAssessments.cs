namespace ITI_GProject.Services
{
    public interface IAssessments
    {

       public Task<IEnumerable<AssDTO>> GetAllAssessments();

      public  Task<AssDTO?> GetAssessmetById(int id);
        public Task<AssDTO> CreateNewAssessment(CreateNewDTO dto);
        public Task<AssDTO?> UpdateAssessment(int id ,UpdateAssDTO updateAssDTO);
        public Task<bool> DeleteAssessment(int id);
        public Task<IEnumerable<AssDTO>> GetAssessmentByLessonId(int lessId);
        public Task<bool> ExistedAssessment(int id);





    }
}
