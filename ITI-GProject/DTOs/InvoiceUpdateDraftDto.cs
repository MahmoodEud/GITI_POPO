namespace ITI_GProject.DTOs
{
    public class InvoiceUpdateDraftDto
    {
        public int? StudentId { get; set; }      
        public int? CourseId { get; set; }        
        public decimal? Amount { get; set; }      
        public string? PaymentRef { get; set; }   
        public string? Notes { get; set; }
        public DateTime? AccessStart { get; set; }
        public DateTime? AccessEnd { get; set; }

    }
}
