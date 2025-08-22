namespace ITI_GProject.DTOs
{
    public class InvoiceSearchFilter
    {
        public int? StudentId { get; set; }
        public int? CourseId { get; set; }
        public InvoiceStatus? Status { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
