using ITI_GProject.Data.HelpingHands;
using Microsoft.AspNetCore.Identity;

namespace ITI_GProject.Data.Models
{
    public class ApplicationUser:IdentityUser
    {

        public DateTime? CreatedAt { get; set; }= DateTime.Now;

        public UserType Type { get; set; }



    }
}
