namespace ITI_GProject.Data.Models
{
    public class StudentCourse
    {


        public int Student_Id { get; set; } 
        public int Course_Id { get; set; }  

        public bool? Subscription { get;set; }
        public virtual Student Student { get; set; } = null;
        public virtual Course Course { get; set; } = null;

    }
}
