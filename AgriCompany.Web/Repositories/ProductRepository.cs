using AgriCompany.Web.Data;
using AgriCompany.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Product> QueryWithDetails()
        {
            return _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images)
                .Include(p => p.Sizes)
                .AsQueryable();
        }

        public async Task<Product?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .Include(p => p.Sizes.OrderBy(s => s.DisplayOrder))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
                .Include(p => p.Sizes.OrderBy(s => s.DisplayOrder))
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            return await _dbSet.AnyAsync(p => p.Code == code && (excludeId == null || p.Id != excludeId));
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
        {
            return await _dbSet.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId));
        }
    }
}
