using AgriCompany.Web.Models;

namespace AgriCompany.Web.Services
{
    public interface IHomepageService
    {
        Task<List<HomepageSlide>> GetActiveSlidesAsync();
        Task<List<HomepageSlide>> GetAllSlidesAsync();
        Task<HomepageSlide?> GetByIdAsync(int id);
        Task CreateAsync(HomepageSlide slide, IFormFile? image);
        Task<bool> UpdateAsync(HomepageSlide slide, IFormFile? image);
        Task<bool> DeleteAsync(int id);

        /// <summary>Uploads slide images still stored under wwwroot to Cloudinary and points the slides at them.</summary>
        Task<SlideImageMoveResult> MoveLocalImagesToCloudinaryAsync();
    }

    public record SlideImageMoveResult(bool CloudinaryEnabled, int Moved, List<string> Failed)
    {
        public static SlideImageMoveResult Disabled() => new(false, 0, new List<string>());
    }
}
