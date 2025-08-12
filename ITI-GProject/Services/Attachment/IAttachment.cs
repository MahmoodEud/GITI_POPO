namespace ITI_GProject.Services.Attachment
{
    public interface IAttachment
    {
        public Task<bool> DeleteAsync(string webPath);
        Task<string> UploadAsync(IFormFile formFile, string folderName);
    }
}
