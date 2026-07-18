using AgriCompany.Web.Data;
using AgriCompany.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetBySlugAsync(string slug) =>
            await _dbSet.FirstOrDefaultAsync(c => c.Slug == slug);

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null) =>
            await _dbSet.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId));

        public async Task<bool> HasProductsAsync(int categoryId) =>
            await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}
