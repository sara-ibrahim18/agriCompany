
using AgriCompany.Web.Models;
using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUploadService _fileUploadService;
        private readonly ISlugService _slugService;

        public ProductService(IUnitOfWork unitOfWork, IFileUploadService fileUploadService, ISlugService slugService)
        {
            _unitOfWork = unitOfWork;
            _fileUploadService = fileUploadService;
            _slugService = slugService;
        }


        public async Task<PagedResult<Product>> GetPublicProductsAsync(
           string? searchTerm,
           int? categoryId,
           string sortBy,
           bool onlyAvailable,
           int pageNumber,
           int pageSize = 12)
        {
            var query = _unitOfWork.Products.QueryWithDetails()
                        .Include(p => p.Sizes)
                        .AsQueryable();   // ← مهم

            if (onlyAvailable)
                query = query.Where(p => p.IsAvailable);

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // البحث مقصور على اسم المنتج (عربي/إنجليزي) واسم القسم فقط،
                // عشان نتايج البحث تكون دقيقة وسريعة ومفيهاش ضغطات زيادة على المستخدم.
                var term = searchTerm.Trim().ToLower();
                query = query.Where(p =>
    p.NameAr.ToLower().Contains(term) ||
    p.NameEn.ToLower().Contains(term) ||
    (p.Category != null && p.Category.NameAr.ToLower().Contains(term)) ||
    (p.Category != null && p.Category.NameEn.ToLower().Contains(term)));
            }

            // Sorting
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Sizes.Min(s => s.Price)),
                "price_desc" => query.OrderByDescending(p => p.Sizes.Min(s => s.Price)),
                "name_asc" => query.OrderBy(p => p.NameAr),
                "name_desc" => query.OrderByDescending(p => p.NameAr),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedResult<Product>.Create(items, pageNumber, pageSize, totalItems);
        }
        public async Task<Product?> GetBySlugAsync(string slug)
        {
            var product = await _unitOfWork.Products.GetBySlugAsync(slug);
            if (product != null)
            {
                product.ViewsCount += 1;
                _unitOfWork.Products.Update(product);
                await _unitOfWork.CompleteAsync();
            }
            return product;
        }

        public async Task<List<Product>> GetFeaturedAsync(int count = 8)
        {
            return await _unitOfWork.Products.QueryWithDetails()
                .Where(p => p.IsFeatured && p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Product>> GetRelatedAsync(int productId, int categoryId, int count = 4)
        {
            return await _unitOfWork.Products.QueryWithDetails()
                .Where(p => p.CategoryId == categoryId && p.Id != productId && p.IsAvailable)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<PagedResult<Product>> GetForAdminAsync(string? searchTerm, int? categoryId, int pageNumber, int pageSize = 15)
        {
            var query = _unitOfWork.Products.QueryWithDetails();

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();
                query = query.Where(p => p.NameAr.Contains(term) || p.NameEn.Contains(term) || p.Code.Contains(term));
            }

            query = query.OrderByDescending(p => p.CreatedAt);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return PagedResult<Product>.Create(items, pageNumber, pageSize, totalItems);
        }

        public async Task<Product?> GetForEditAsync(int id) => await _unitOfWork.Products.GetWithDetailsAsync(id);

        public async Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile>? images, List<ProductSize> sizes)
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(product.NameAr))
                //    return (false, "اسم المنتج بالعربية مطلوب");

                //if (string.IsNullOrWhiteSpace(product.Code))
                //    return (false, "كود المنتج مطلوب");

                //if (product.CategoryId <= 0)
                //    return (false, "يجب اختيار قسم");

                //if (await _unitOfWork.Products.CodeExistsAsync(product.Code))
                //    return (false, "كود المنتج مستخدم بالفعل");
                if (string.IsNullOrWhiteSpace(product.NameAr))
                    return (false, "اسم المنتج بالعربية مطلوب");

                if (product.CategoryId <= 0)
                    return (false, "يجب اختيار قسم");

                // الكود بيتولد تلقائيًا، الأدمن مش محتاج يكتبه
                product.Code = await GenerateUniqueCodeAsync();

                // Slug - حتى لو NameEn فاضي
                // Slug - حتى لو NameEn فاضي
                var slugSource = !string.IsNullOrWhiteSpace(product.NameEn) ? product.NameEn : product.NameAr;
                product.Slug = await GenerateUniqueSlugAsync(slugSource, null);
                product.CreatedAt = DateTime.Now;

                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CompleteAsync();

                // Images
                if (images != null && images.Count > 0)
                {
                    var isFirst = true;
                    foreach (var image in images.Where(i => i.Length > 0))
                    {
                        var path = await _fileUploadService.SaveImageAsync(image, "products");
                        await _unitOfWork.ProductImages.AddAsync(new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = path,
                            IsMain = isFirst,
                            DisplayOrder = 0
                        });
                        isFirst = false;
                    }
                    await _unitOfWork.CompleteAsync();
                }

                // Sizes
                if (sizes != null && sizes.Any())
                {
                    var order = 0;
                    foreach (var size in sizes)
                    {
                        size.ProductId = product.Id;
                        size.DisplayOrder = order++;
                        await _unitOfWork.ProductSizes.AddAsync(size);
                    }
                    await _unitOfWork.CompleteAsync();
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, "خطأ: " + ex.Message);
            }
        }
        //public async Task<(bool Success, string? Error)> CreateAsync(Product product, List<IFormFile>? images, List<ProductSize> sizes)
        //{
        //    if (await _unitOfWork.Products.CodeExistsAsync(product.Code))
        //        return (false, "كود المنتج مستخدم بالفعل، من فضلك استخدم كود آخر");

        //    // Use NameEn if provided, otherwise NameAr for slug
        //    var slugSource = !string.IsNullOrWhiteSpace(product.NameEn) ? product.NameEn : product.NameAr;
        //    product.Slug = await GenerateUniqueSlugAsync(slugSource, null); product.CreatedAt = DateTime.Now;

        //    await _unitOfWork.Products.AddAsync(product);

        //    await _unitOfWork.CompleteAsync();   // Save product first to get Id

        //    // Upload images
        //    if (images != null && images.Count > 0)
        //    {
        //        var isFirst = true;
        //        try
        //        {
        //            foreach (var image in images.Where(i => i.Length > 0))
        //            {
        //                var path = await _fileUploadService.SaveImageAsync(image, "products");
        //                await _unitOfWork.ProductImages.AddAsync(new ProductImage
        //                {
        //                    ProductId = product.Id,
        //                    ImagePath = path,
        //                    IsMain = isFirst,
        //                    DisplayOrder = 0
        //                });
        //                isFirst = false;
        //            }
        //            await _unitOfWork.CompleteAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            _unitOfWork.Products.Remove(product);
        //            await _unitOfWork.CompleteAsync();
        //            return (false, ex.Message);
        //        }
        //    }

        //    // Add sizes with prices
        //    if (sizes != null && sizes.Any())
        //    {
        //        var order = 0;
        //        foreach (var size in sizes)
        //        {
        //            size.ProductId = product.Id;
        //            size.DisplayOrder = order++;
        //            await _unitOfWork.ProductSizes.AddAsync(size);
        //        }
        //        await _unitOfWork.CompleteAsync();
        //    }

        //    return (true, null);
        //}

        public async Task<(bool Success, string? Error)> UpdateAsync(Product product, List<IFormFile>? newImages, List<ProductSize> sizes)
        {
            var existing = await _unitOfWork.Products.GetWithDetailsAsync(product.Id);
            if (existing == null)
                return (false, "المنتج غير موجود");

            if (await _unitOfWork.Products.CodeExistsAsync(product.Code, product.Id))
                return (false, "كود المنتج مستخدم بالفعل، من فضلك استخدم كود آخر");



            if (!string.Equals(existing.NameEn, product.NameEn, StringComparison.OrdinalIgnoreCase))
            {
                var oldSlug = existing.Slug;

                // Use NameEn if provided, otherwise NameAr for slug
                var slugSource = !string.IsNullOrWhiteSpace(product.NameEn) ? product.NameEn : product.NameAr;
                existing.Slug = await GenerateUniqueSlugAsync(slugSource, existing.Id);

                if (!string.IsNullOrWhiteSpace(oldSlug) && !string.Equals(oldSlug, existing.Slug, StringComparison.OrdinalIgnoreCase))
                {
                    await _unitOfWork.ProductSlugHistories.AddAsync(new ProductSlugHistory
                    {
                        OldSlug = oldSlug,
                        ProductId = existing.Id
                    });
                }
            }

            existing.NameAr = product.NameAr;
            existing.NameEn = product.NameEn;
            existing.Code = product.Code;
            existing.Description = product.Description;
            existing.Benefits = product.Benefits;
            existing.UsageInstructions = product.UsageInstructions;
            //existing.Price = product.Price;           // السعر العام (اختياري)
            //existing.Unit = product.Unit;
            existing.CategoryId = product.CategoryId;
            existing.IsAvailable = product.IsAvailable;
            existing.IsFeatured = product.IsFeatured;
            existing.DisplayOrder = product.DisplayOrder;
            existing.UpdatedAt = DateTime.Now;

            _unitOfWork.Products.Update(existing);

            // Update images
            if (newImages != null && newImages.Count > 0)
            {
                var hasMain = existing.Images.Any(i => i.IsMain);
                try
                {
                    foreach (var image in newImages.Where(i => i.Length > 0))
                    {
                        var path = await _fileUploadService.SaveImageAsync(image, "products");
                        await _unitOfWork.ProductImages.AddAsync(new ProductImage
                        {
                            ProductId = existing.Id,
                            ImagePath = path,
                            IsMain = !hasMain,
                            DisplayOrder = existing.Images.Count
                        });
                        hasMain = true;
                    }
                }
                catch (Exception ex)
                {
                    return (false, ex.Message);
                }
            }

            // Update sizes (delete old + add new with prices)
            foreach (var oldSize in existing.Sizes.ToList())
            {
                _unitOfWork.ProductSizes.Remove(oldSize);
            }

            if (sizes != null && sizes.Any())
            {
                var order = 0;
                foreach (var size in sizes)
                {
                    size.ProductId = existing.Id;
                    size.DisplayOrder = order++;
                    await _unitOfWork.ProductSizes.AddAsync(size);
                }
            }

            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetWithDetailsAsync(id);
            if (product == null) return false;

            foreach (var image in product.Images)
            {
                _fileUploadService.DeleteImage(image.ImagePath);
            }

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> ToggleAvailabilityAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return false;

            product.IsAvailable = !product.IsAvailable;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task DeleteImageAsync(int imageId)
        {
            var image = await _unitOfWork.ProductImages.GetByIdAsync(imageId);
            if (image == null) return;

            _fileUploadService.DeleteImage(image.ImagePath);
            _unitOfWork.ProductImages.Remove(image);
            await _unitOfWork.CompleteAsync();
        }

        public async Task SetMainImageAsync(int productId, int imageId)
        {
            var images = await _unitOfWork.ProductImages.FindAsync(i => i.ProductId == productId);
            foreach (var img in images)
            {
                img.IsMain = img.Id == imageId;
                _unitOfWork.ProductImages.Update(img);
            }
            await _unitOfWork.CompleteAsync();
        }

        //private async Task<string> GenerateUniqueSlugAsync(string nameEn, int? excludeId)
        //{
        //    var baseSlug = _slugService.GenerateSlug(nameEn);
        //    var slug = baseSlug;
        //    var counter = 1;
        //    while (await _unitOfWork.Products.SlugExistsAsync(slug, excludeId))
        //    {
        //        slug = $"{baseSlug}-{counter}";
        //        counter++;
        //    }
        //    return slug;
        //}
        private async Task<string> GenerateUniqueSlugAsync(string input, int? excludeId)
        {
            try
            {
                var baseSlug = _slugService.GenerateSlug(input);
                var slug = baseSlug;
                var counter = 1;

                while (await _unitOfWork.Products.SlugExistsAsync(slug, excludeId))
                {
                    slug = $"{baseSlug}-{counter}";
                    counter++;
                }

                return slug;
            }
            catch
            {
                return "product-" + DateTime.Now.Ticks.ToString().Substring(8);
            }
        }
        private async Task<string> GenerateUniqueCodeAsync()
        {
            var random = new Random();
            string code;
            var attempts = 0;

            do
            {
                code = $"PRD-{DateTime.Now:yyMMdd}{random.Next(100, 999)}";
                attempts++;
            }
            while (await _unitOfWork.Products.CodeExistsAsync(code) && attempts < 20);

            return code;
        }
        public async Task<string?> GetCurrentSlugFromOldSlugAsync(string oldSlug)
        {
            var history = await _unitOfWork.ProductSlugHistories.FirstOrDefaultAsync(h => h.OldSlug == oldSlug);
            if (history == null) return null;

            var product = await _unitOfWork.Products.GetByIdAsync(history.ProductId);
            return product?.Slug;
        }
    }
}
