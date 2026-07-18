using AgriCompany.Web.Models;

namespace AgriCompany.Web.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<bool> HasProductsAsync(int categoryId);
    }
}
