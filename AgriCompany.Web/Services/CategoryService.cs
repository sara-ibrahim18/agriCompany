//using AgriCompany.Web.Models;
//using AgriCompany.Web.Models.ViewModels;
//using AgriCompany.Web.Repositories;
//using Microsoft.EntityFrameworkCore;

//namespace AgriCompany.Web.Services
//{
//    public class CategoryService : ICategoryService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IFileUploadService _fileUploadService;
//        private readonly ISlugService _slugService;

//        public CategoryService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, ISlugService slugService)
//        {
//            _unitOfWork = unitOfWork;
//            _fileUploadService = fileUploadService;
//            _slugService = slugService;
//        }

//        public async Task<List<Category>> GetActiveAsync() =>
//            (await _unitOfWork.Categories.FindAsync(c => c.IsActive))
//                .OrderBy(c => c.DisplayOrder).ToList();

//        public async Task<List<Category>> GetAllAsync() =>
//            (await _unitOfWork.Categories.GetAllAsync()).OrderBy(c => c.DisplayOrder).ToList();

//        public async Task<PagedResult<Category>> GetPagedAsync(int pageNumber, int pageSize = 15)
//        {
//            var query = _unitOfWork.Categories.Query().OrderBy(c => c.DisplayOrder);
//            var totalItems = await query.CountAsync();
//            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
//            return PagedResult<Category>.Create(items, pageNumber, pageSize, totalItems);
//        }

//        public async Task<Category?> GetByIdAsync(int id) => await _unitOfWork.Categories.GetByIdAsync(id);

//        public async Task<Category?> GetBySlugAsync(string slug) => await _unitOfWork.Categories.GetBySlugAsync(slug);

//        public async Task<(bool Success, string? Error)> CreateAsync(Category category, IFormFile? image)
//        {
//            category.Slug = await GenerateUniqueSlugAsync(category.NameEn, null);
//            category.CreatedAt = DateTime.Now;

//            if (image != null && image.Length > 0)
//            {
//                category.ImagePath = await _fileUploadService.SaveImageAsync(image, "categories");
//            }

//            await _unitOfWork.Categories.AddAsync(category);
//            await _unitOfWork.CompleteAsync();
//            return (true, null);
//        }

//        public async Task<(bool Success, string? Error)> UpdateAsync(Category category, IFormFile? image)
//        {
//            var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
//            if (existing == null)
//                return (false, "القسم غير موجود");

//            if (!string.Equals(existing.NameEn, category.NameEn, StringComparison.OrdinalIgnoreCase))
//            {
//                existing.Slug = await GenerateUniqueSlugAsync(category.NameEn, existing.Id);
//            }

//            existing.NameAr = category.NameAr;
//            existing.NameEn = category.NameEn;
//            existing.Description = category.Description;
//            existing.IsActive = category.IsActive;
//            existing.DisplayOrder = category.DisplayOrder;

//            if (image != null && image.Length > 0)
//            {
//                _fileUploadService.DeleteImage(existing.ImagePath);
//                existing.ImagePath = await _fileUploadService.SaveImageAsync(image, "categories");
//            }

//            _unitOfWork.Categories.Update(existing);
//            await _unitOfWork.CompleteAsync();
//            return (true, null);
//        }

//        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
//        {
//            if (await _unitOfWork.Categories.HasProductsAsync(id))
//                return (false, "لا يمكن حذف القسم لأنه يحتوي على منتجات، قم بحذف أو نقل المنتجات أولاً");

//            var category = await _unitOfWork.Categories.GetByIdAsync(id);
//            if (category == null)
//                return (false, "القسم غير موجود");

//            _fileUploadService.DeleteImage(category.ImagePath);
//            _unitOfWork.Categories.Remove(category);
//            await _unitOfWork.CompleteAsync();
//            return (true, null);
//        }

//        private async Task<string> GenerateUniqueSlugAsync(string nameEn, int? excludeId)
//        {
//            var baseSlug = _slugService.GenerateSlug(nameEn);
//            var slug = baseSlug;
//            var counter = 1;
//            while (await _unitOfWork.Categories.SlugExistsAsync(slug, excludeId))
//            {
//                slug = $"{baseSlug}-{counter}";
//                counter++;
//            }
//            return slug;
//        }
//    }
//}
using AgriCompany.Web.Models;
using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISlugService _slugService;

        public CategoryService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, ISlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _slugService = slugService;
        }

        public async Task<List<Category>> GetActiveAsync() =>
            (await _unitOfWork.Categories.FindAsync(c => c.IsActive))
                .OrderBy(c => c.DisplayOrder).ToList();

        public async Task<List<Category>> GetAllAsync() =>
            (await _unitOfWork.Categories.GetAllAsync()).OrderBy(c => c.DisplayOrder).ToList();

        public async Task<PagedResult<Category>> GetPagedAsync(int pageNumber, int pageSize = 15)
        {
            var query = _unitOfWork.Categories.Query().OrderBy(c => c.DisplayOrder);
            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return PagedResult<Category>.Create(items, pageNumber, pageSize, totalItems);
        }

        public async Task<Category?> GetByIdAsync(int id) => await _unitOfWork.Categories.GetByIdAsync(id);

        public async Task<Category?> GetBySlugAsync(string slug) => await _unitOfWork.Categories.GetBySlugAsync(slug);

        public async Task<(bool Success, string? Error)> CreateAsync(Category category, IFormFile? image)
        {
            category.Slug = await GenerateUniqueSlugAsync(category.NameEn, null);
            category.CreatedAt = DateTime.Now;

            if (image != null && image.Length > 0)
            {
                try
                {
                    category.ImagePath = await _fileUploadService.SaveImageAsync(image, "categories");
                }
                catch (ArgumentException ex)
                {
                    return (false, ex.Message);
                }
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(Category category, IFormFile? image)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
            if (existing == null)
                return (false, "القسم غير موجود");

            if (!string.Equals(existing.NameEn, category.NameEn, StringComparison.OrdinalIgnoreCase))
            {
                existing.Slug = await GenerateUniqueSlugAsync(category.NameEn, existing.Id);
            }

            existing.NameAr = category.NameAr;
            existing.NameEn = category.NameEn;
            existing.Description = category.Description;
            existing.IsActive = category.IsActive;
            existing.DisplayOrder = category.DisplayOrder;

            if (image != null && image.Length > 0)
            {
                try
                {
                    var newPath = await _fileUploadService.SaveImageAsync(image, "categories");
                    _fileUploadService.DeleteImage(existing.ImagePath);
                    existing.ImagePath = newPath;
                }
                catch (ArgumentException ex)
                {
                    return (false, ex.Message);
                }
            }

            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            if (await _unitOfWork.Categories.HasProductsAsync(id))
                return (false, "لا يمكن حذف القسم لأنه يحتوي على منتجات، قم بحذف أو نقل المنتجات أولاً");

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return (false, "القسم غير موجود");

            _fileUploadService.DeleteImage(category.ImagePath);
            _unitOfWork.Categories.Remove(category);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        private async Task<string> GenerateUniqueSlugAsync(string nameEn, int? excludeId)
        {
            var baseSlug = _slugService.GenerateSlug(nameEn);
            var slug = baseSlug;
            var counter = 1;
            while (await _unitOfWork.Categories.SlugExistsAsync(slug, excludeId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }
            return slug;
        }
    }
}