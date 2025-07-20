namespace ITI_GProject.Data.Models
{
    public class StudentQuiz
    {

        public int Student_Id { get; set; }
        public int Quiz_Id { get; set; }

        public int? TotAl_Marks { get; set; }

        public virtual Student Student { get; set; } = null;
        public virtual Quiz Quiz { get; set; } = null;
    }
}
