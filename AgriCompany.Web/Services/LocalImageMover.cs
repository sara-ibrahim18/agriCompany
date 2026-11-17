namespace AgriCompany.Web.Services
{
    /// <summary>
    /// Re-uploads images saved under wwwroot before the switch to Cloudinary (paths like /images/products/x.jpg).
    /// The old file is kept on the server on purpose, as a backup.
    /// </summary>
    public class LocalImageMover
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<LocalImageMover> _logger;

        public LocalImageMover(IFileUploadService fileUploadService, IWebHostEnvironment env, ILogger<LocalImageMover> logger)
        {
            _fileUploadService = fileUploadService;
            _env = env;
            _logger = logger;
        }

        public bool CloudinaryEnabled => _fileUploadService is CloudinaryUploadService;

        /// <summary>True for paths stored on this server rather than a full (Cloudinary) URL.</summary>
        public static bool IsLocal(string? path) => !string.IsNullOrWhiteSpace(path) && path.StartsWith('/');

        /// <summary>Returns the new Cloudinary URL, or an Arabic error message for the admin.</summary>
        public async Task<(string? NewPath, string? Error)> MoveAsync(string localPath, string folder)
        {
            var fullPath = Path.Combine(_env.WebRootPath, localPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath))
                return (null, "الصورة مش موجودة على السيرفر");

            try
            {
                await using var stream = File.OpenRead(fullPath);
                var file = new FormFile(stream, 0, stream.Length, "image", Path.GetFileName(fullPath));
                return (await _fileUploadService.SaveImageAsync(file, folder), null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Moving {Path} to Cloudinary failed", localPath);
                return (null, ex is ArgumentException ? ex.Message : "تعذر رفع الصورة");
            }
        }
    }

    public record ImageMoveResult(bool CloudinaryEnabled, int Moved, int Remaining, List<string> Failed)
    {
        public static ImageMoveResult Disabled() => new(false, 0, 0, new List<string>());
    }
}
