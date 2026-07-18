//using AgriCompany.Web.Models;
//using AgriCompany.Web.Models.ViewModels;

//namespace AgriCompany.Web.Services
//{
//    public interface IProductService
//    {
//        Task<PagedResult<Product>> GetPublicProductsAsync(string? searchTerm, int? categoryId, string sortBy, bool onlyAvailable, int pageNumber, int pageSize = 12);
//        Task<Product?> GetBySlugAsync(string slug);
//        Task<List<Product>> GetFeaturedAsync(int count = 8);
//        Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);

//        Task<PagedResult<Product>> GetForAdminAsync(string? searchTerm, int? categoryId, int pageNumber, int pageSize = 15);
//        Task<Product?> GetForEditAsync(int id);
//        //Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile>? images, List<string>? sizeNames);
//        Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile> images, List<ProductSize> sizes);
//        Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<string>? sizeNames);
//        Task<bool> DeleteAsync(int id);
//        Task<bool> ToggleAvailabilityAsync(int id);
//        Task DeleteImageAsync(int imageId);
//        Task SetMainImageAsync(int productId, int imageId);
//        //Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);
//        //Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);
//        //Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);
//        Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);
//    }
//}
//using AgriCompany.Web.Models;
//using AgriCompany.Web.Models.ViewModels;

//namespace AgriCompany.Web.Services
//{
//    public interface IProductService
//    {
//        Task<PagedResult<Product>> GetPublicProductsAsync(string? searchTerm, int? categoryId, string sortBy, bool onlyAvailable, int pageNumber, int pageSize = 12);
//        Task<Product?> GetBySlugAsync(string slug);
//        Task<List<Product>> GetFeaturedAsync(int count = 8);
//        Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);
//        Task<PagedResult<Product>> GetForAdminAsync(string? searchTerm, int? categoryId, int pageNumber, int pageSize = 15);
//        Task<Product?> GetForEditAsync(int id);

//        Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile> images, List<ProductSize> sizes);
//        Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);

//        Task<bool> DeleteAsync(int id);
//        Task<bool> ToggleAvailabilityAsync(int id);
//        Task DeleteImageAsync(int imageId);
//        Task SetMainImageAsync(int productId, int imageId);
//    }
//}
using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Models;

namespace AgriCompany.Web.Services
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetPublicProductsAsync(string? searchTerm, int? categoryId, string sortBy, bool onlyAvailable, int pageNumber, int pageSize = 12);
        Task<Product?> GetBySlugAsync(string slug);
        Task<string?> GetCurrentSlugFromOldSlugAsync(string oldSlug);
        Task<List<Product>> GetFeaturedAsync(int count = 8);
        Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4);
        Task<PagedResult<Product>> GetForAdminAsync(string? searchTerm, int? categoryId, int pageNumber, int pageSize = 15);
        Task<Product?> GetForEditAsync(int id);

        Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile> images, List<ProductSize> sizes);
        Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes);

        Task<bool> DeleteAsync(int id);
        Task<bool> ToggleAvailabilityAsync(int id);
        Task DeleteImageAsync(int imageId);
        Task SetMainImageAsync(int productId, int imageId);
    }
}