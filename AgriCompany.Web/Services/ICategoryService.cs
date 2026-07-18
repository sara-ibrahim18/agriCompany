using AgriCompany.Web.Models;
using AgriCompany.Web.Models.ViewModels;

namespace AgriCompany.Web.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetActiveAsync();
        Task<List<Category>> GetAllAsync();
        Task<PagedResult<Category>> GetPagedAsync(int pageNumber, int pageSize = 15);
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<(bool Success, string? Error)> CreateAsync(Category category, IFormFile? image);
        Task<(bool Success, string? Error)> UpdateAsync(Category category, IFormFile? image);
        Task<(bool Success, string? Error)> DeleteAsync(int id);
    }
}
