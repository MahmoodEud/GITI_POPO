
namespace ITI_GProject.Services.Attachment
{
    public class Attachments : IAttachment
    {
        private readonly IWebHostEnvironment _env;

        public Attachments(IWebHostEnvironment env)
        {
            _env = env;
        }

        List<string> AllowExtentions = [".jpg", ".jpeg",".png"];

        int maxSize = 2 * 1024 * 1024;
        public Task<bool> DeleteAsync(string webPath)
        {
            if (string.IsNullOrWhiteSpace(webPath)) return Task.FromResult(false);

            var relative = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var physical = Path.Combine(_env.WebRootPath, relative);

            if (!File.Exists(physical)) return Task.FromResult(false);

            try { File.Delete(physical); return Task.FromResult(true); }
            catch { return Task.FromResult(false); }
        }

        public async Task<string?> UploadAsync(IFormFile file, string folderName)
        {
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!AllowExtentions.Contains(ext)) return null;
            if (file.Length > maxSize || file.Length == 0) return null;

            var folderPath = Path.Combine(_env.WebRootPath, "Files", folderName);
            Directory.CreateDirectory(folderPath); 

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folderPath, fileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return $"/Files/{folderName}/{fileName}";
        }

   
    }
}
