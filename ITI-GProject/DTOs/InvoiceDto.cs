namespace ITI_GProject.DTOs
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string? InvoiceNo { get; set; }

        public int StudentId { get; set; }
        public string? StudentName { get; set; }

        public int? CourseId { get; set; }
        public string? CourseTitle { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

        public InvoiceStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? PaymentRef { get; set; }

        public string? Notes { get; set; }
        public string? AttachmentUrl { get; set; }

        public DateTime? AccessStart { get; set; }
        public DateTime? AccessEnd { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedByUserId { get; set; }
    }
}
