namespace ITI_GProject.Data.Models
{
    public class StudentAssignment
    {
        public int Studet_Id { get; set; }
        public int Assignemt_Id { get; set; }

        public virtual Student Student { get; set; } = null;

        public virtual Assignment Assignment { get; set; } = null;




    }
}
