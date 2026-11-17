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
        Task<ImageMoveResult> MoveLocalImagesToCloudinaryAsync();
    }
}
