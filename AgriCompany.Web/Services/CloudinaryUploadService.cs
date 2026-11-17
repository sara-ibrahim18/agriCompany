using System.Text.RegularExpressions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AgriCompany.Web.Services
{
    /// <summary>
    /// Uploads images to Cloudinary and returns the full https URL.
    /// Images uploaded before the switch (paths like /images/...) are still deleted from wwwroot.
    /// </summary>
    public class CloudinaryUploadService : IFileUploadService
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif", ".svg" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private const string RootFolder = "elbrakah";

        private readonly Cloudinary _cloudinary;
        private readonly FileUploadService _localFiles;
        private readonly ILogger<CloudinaryUploadService> _logger;

        public CloudinaryUploadService(Cloudinary cloudinary, FileUploadService localFiles, ILogger<CloudinaryUploadService> logger)
        {
            _cloudinary = cloudinary;
            _localFiles = localFiles;
            _logger = logger;
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("لم يتم اختيار ملف صورة");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"صيغة الصورة غير مدعومة. الصيغ المسموح بها: {string.Join(", ", AllowedExtensions.Select(e => e.TrimStart('.')))}");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException("حجم الصورة أكبر من الحد المسموح به (5 ميجابايت)");

            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription($"{Guid.NewGuid()}{extension}", stream),
                Folder = $"{RootFolder}/{folder}",
                UseFilename = false,
                UniqueFilename = true,
                Overwrite = false
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            // Callers show ArgumentException messages to the admin, so upload failures use it too.
            if (result.Error != null || result.SecureUrl == null)
            {
                _logger.LogError("Cloudinary upload failed: {Error}", result.Error?.Message);
                throw new ArgumentException("تعذر رفع الصورة، حاول مرة أخرى");
            }

            return result.SecureUrl.ToString();
        }

        public void DeleteImage(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var publicId = GetPublicId(relativePath);
            if (publicId == null)
            {
                // Old image saved on the server before moving to Cloudinary
                _localFiles.DeleteImage(relativePath);
                return;
            }

            try
            {
                var result = _cloudinary.Destroy(new DeletionParams(publicId));
                if (result.Error != null)
                    _logger.LogWarning("Cloudinary delete failed for {PublicId}: {Error}", publicId, result.Error.Message);
            }
            catch (Exception ex)
            {
                // non-critical, same as local delete failures
                _logger.LogWarning(ex, "Cloudinary delete failed for {PublicId}", publicId);
            }
        }

        /// <summary>
        /// Extracts "elbrakah/categories/abc" from
        /// https://res.cloudinary.com/{cloud}/image/upload/v123/elbrakah/categories/abc.jpg.
        /// Returns null for anything that isn't one of our Cloudinary images.
        /// </summary>
        private static string? GetPublicId(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.Host.EndsWith("cloudinary.com", StringComparison.OrdinalIgnoreCase))
                return null;

            const string marker = "/upload/";
            var path = Uri.UnescapeDataString(uri.AbsolutePath);
            var index = path.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0)
                return null;

            var rest = Regex.Replace(path[(index + marker.Length)..], @"^v\d+/", "");
            var extension = Path.GetExtension(rest);
            var publicId = extension.Length > 0 ? rest[..^extension.Length] : rest;

            return publicId.StartsWith(RootFolder + "/", StringComparison.Ordinal) ? publicId : null;
        }
    }
}
