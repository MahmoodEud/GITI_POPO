namespace ITI_GProject.Data.Models
{
    public class ApplicationUser:IdentityUser
    {


        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public DateTime Birthdate { get; set; }

        public DateTime? CreatedAt { get; set; }= DateTime.Now;

        public UserType Type { get; set; }

        public virtual Student? StudentProfile { get; set; }

    }
}
