using ITI_GProject.Data.HelpingHands;
using Microsoft.AspNetCore.Identity;

namespace ITI_GProject.Data.Models
{
    public class ApplicationUser:IdentityUser
    {


        public string Name { get; set; }
        public string Pphone { get; set; }
        public DateTime Birthdate { get; set; }
        public StudentYear StudentYear { get; set; }
        public bool IsApproved { get; set; } = false;

        public DateTime? CreatedAt { get; set; }= DateTime.Now;

        public UserType Type { get; set; }



    }
}
