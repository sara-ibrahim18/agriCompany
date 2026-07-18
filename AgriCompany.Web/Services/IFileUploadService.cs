namespace AgriCompany.Web.Services
{
    public interface IFileUploadService
    {
        /// <summary>Saves an uploaded image under wwwroot/images/{folder}/ and returns its public relative path.</summary>
        Task<string> SaveImageAsync(IFormFile file, string folder);
        void DeleteImage(string? relativePath);
    }
}
