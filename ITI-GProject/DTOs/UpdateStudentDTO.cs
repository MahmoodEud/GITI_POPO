namespace ITI_GProject.DTOs
{
    public class UpdateStudentDTO
    {
        public string Name { get; set; } = default!;
        public string? UserName { get; set; }
        public StudentYear Year { get; set; }
        public string PhoneNumber { get; set; } = default!;
        public string ParentNumber { get; set; } = default!;
        public DateTime? Birthdate { get; set; }

    }
}
