namespace ITI_GProject.DTOs
{
    public class InvoiceAttachmentDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
