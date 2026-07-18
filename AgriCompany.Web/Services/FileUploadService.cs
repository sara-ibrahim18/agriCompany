namespace AgriCompany.Web.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        //public async Task<string> SaveImageAsync(IFormFile file, string folder)
        //{
        //    if (file == null || file.Length == 0)
        //        throw new ArgumentException("لم يتم اختيار ملف صورة");

        //    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        //    if (!AllowedExtensions.Contains(extension))
        //        throw new ArgumentException("صيغة الصورة غير مدعومة. الصيغ المسموح بها: jpg, jpeg, png, webp, gif");

        //    if (file.Length > MaxFileSizeBytes)
        //        throw new ArgumentException("حجم الصورة أكبر من الحد المسموح به (5 ميجابايت)");

        //    var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folder);
        //    Directory.CreateDirectory(uploadsFolder);

        //    var fileName = $"{Guid.NewGuid()}{extension}";
        //    var filePath = Path.Combine(uploadsFolder, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    return $"/images/{folder}/{fileName}";
        //}
        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("لم يتم اختيار ملف صورة");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            // أضف .svg
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException($"صيغة الصورة غير مدعومة. الصيغ المسموح بها: {string.Join(", ", allowedExtensions.Select(e => e.TrimStart('.')))}");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException("حجم الصورة أكبر من الحد المسموح به (5 ميجابايت)");

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{folder}/{fileName}";
        }
        public void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                try { File.Delete(fullPath); } catch { /* non-critical, ignore delete failures */ }
            }
        }
    }
}
