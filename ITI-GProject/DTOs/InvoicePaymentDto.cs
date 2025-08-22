namespace ITI_GProject.DTOs
{
    public class InvoicePaymentDto
    {
        [Required] public PaymentMethod Method { get; set; }
        public string? PaymentRef { get; set; }
        public string? Notes { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
