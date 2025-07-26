namespace ITI_GProject.Data.Models
{
    public class ApplicationUser:IdentityUser
    {


        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public bool IsApproved { get; set; } = false;

        public DateTime? CreatedAt { get; set; }= DateTime.Now;

        public UserType Type { get; set; }

        public virtual Student StudentProfile { get; set; }

    }
}
/*
 {
  "userName": "moo_eid",
  "password": "P@ssw0rd"
}
 {
  "name": "Mahmood Muhammad Eid Abdelgawad",
  "userName": "moo_eid",
  "phone": "01244887364",
  "pphone": "01207818812",
  "birthdate": "2020-07-24",
  "studentYear": 1,
  "password": "P@ssw0rd",
  "confirmPassword": "P@ssw0rd"

}
 
 */