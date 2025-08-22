namespace ITI_GProject.Services
{
    public interface IInvoicesService
    {
        Task<InvoiceDto?> CreateDraftAsync(InvoiceCreateDto dto, string createdByUserId);
        Task<InvoiceDto?> SendAsync(int invoiceId, string updatedByUserId);
        Task<InvoiceDto?> MarkPaidAsync(int invoiceId, InvoicePaymentDto payment, string updatedByUserId); 
        Task<InvoiceDto?> CancelAsync(int invoiceId, string? reason, string updatedByUserId);

        Task<InvoiceDto?> UploadAttachmentAsync(int invoiceId, IFormFile file, string updatedByUserId);

        Task<InvoiceDto?> GetByIdAsync(int id);
        Task<PagedResult<InvoiceDto>> SearchAsync(InvoiceSearchFilter filter); 
        Task<IReadOnlyList<InvoiceDto>> GetByStudentAsync(int studentId);
        Task<InvoiceDto?> UpdateDraftAsync(int id, InvoiceUpdateDraftDto dto, string updatedByUserId);
        Task<bool> DeleteDraftAsync(int id, string userId);
        Task<IReadOnlyList<InvoiceDto>> GetMineAsync(string userId);          
        Task<InvoiceDto?> GetMineByIdAsync(string userId, int invoiceId);
    }
}
