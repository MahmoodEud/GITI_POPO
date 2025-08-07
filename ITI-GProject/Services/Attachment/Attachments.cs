
namespace ITI_GProject.Services.Attachment
{
    public class Attachments : IAttachment
    {
        List<string> AllowExtentions = [".jpg", ".jpeg",".png"];

        int maxSize = 2 * 1024 * 1024;
        public bool Delete(string FilePath)
        {
            if(FilePath is null|| !File.Exists(FilePath)) return false;

            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch
            { 
                return false;
            }
        }

        public string? Uplaod(IFormFile file, string FolderName)
        {
            var extansion = Path.GetExtension(file.FileName).ToLower();

            if(!AllowExtentions.Contains(extansion))
            {
                return null;
            }
            if(file.Length > maxSize || file.Length == 0)
            {
                return null;
            }

            var folderName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files",FolderName);
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderName, fileName);

            using FileStream fileStream = new FileStream(filePath, FileMode.Create);

            file.CopyTo(fileStream);

            return fileName;
        }
    }
}
