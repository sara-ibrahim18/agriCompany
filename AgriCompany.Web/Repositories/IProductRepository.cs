using AgriCompany.Web.Models;

namespace AgriCompany.Web.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product?> GetWithDetailsAsync(int id);
        Task<Product?> GetBySlugAsync(string slug);
        Task<bool> CodeExistsAsync(string code, int? excludeId = null);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        IQueryable<Product> QueryWithDetails();
    }
}
