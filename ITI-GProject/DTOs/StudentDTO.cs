namespace ITI_GProject.DTOs
{
    public class StudentDTO
    {
        public string Id { get; set; } = default!;
        public int StudentId { get; set; }
        public string Name { get; set; } = default!;
        public string? UserName { get; set; }
        public StudentYear Year { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string ParentNumber { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; }
        public DateTime Birthdate { get; set; }
        public string StudentLevel { get; set; } = default!;
        public string Role { get; set; } = default!;


    }
}
