namespace ITI_GProject.Services.Attachment
{
    public interface IAttachment
    {
        bool Delete(string FilePath);
        string Uplaod(IFormFile formFile, string folderName);
    }
}
