namespace ITI_GProject.DTOs
{
    public class InvoiceCreateDto
    {
        [Required] public int StudentId { get; set; }
        public int? CourseId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [MaxLength(8)]
        public string Currency { get; set; } = "EGP";

        public string? Notes { get; set; }
        public string? PaymentRef { get; set; }
        public DateTime? AccessStart { get; set; }
        public DateTime? AccessEnd { get; set; }
    }
}
