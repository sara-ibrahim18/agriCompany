using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? search, int? categoryId, int page = 1)
        {
            var paged = await _productService.GetForAdminAsync(search, categoryId, page);
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.LocalImageCount = await _productService.CountLocalImagesAsync();
            return View(paged);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(new Product());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> images,
    List<string> SizeName, List<decimal> SizePrice)
        {
            //// حل قوي لمشكلة NameEn
            //ModelState.Remove("Slug");
            //ModelState.Remove("NameEn");

            //if (ModelState.ContainsKey("NameEn"))
            //{
            //    ModelState["NameEn"]!.Errors.Clear();
            //}

            //// لو لسه فيه أخطاء تانية، هنشوفها
            //if (!ModelState.IsValid)
            //{
            //    // Debug مؤقت: نجمع كل الأخطاء الحقيقية عشان نعرف السبب بالظبط
            //    var errors = ModelState
            //        .Where(kvp => kvp.Value?.Errors.Count > 0)
            //        .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value!.Errors.Select(e => e.ErrorMessage))}")
            //        .ToList();
            //    ViewBag.DebugErrors = errors;

            //    ViewBag.Categories = await _categoryService.GetAllAsync();
            //    return View(product);
            //}

          
            ModelState.Remove("Slug");
            ModelState.Remove("NameEn");
            ModelState.Remove("Code");

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(product);
            }
            var sizes = new List<ProductSize>();
            for (int i = 0; i < SizeName.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(SizeName[i]))
                {
                    sizes.Add(new ProductSize
                    {
                        SizeName = SizeName[i],
                        Price = SizePrice[i],
                        DisplayOrder = i,
                        IsAvailable = true
                    });
                }
            }

            var (success, error) = await _productService.CreateAsync(product, images, sizes);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "حدث خطأ أثناء الحفظ");
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(product);
            }

            TempData["Success"] = "تم إضافة المنتج بنجاح";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetForEditAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(product);
        }

 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, List<IFormFile> newImages,
        List<string> SizeName, List<decimal> SizePrice, List<string> SizeAvailable)
        {
            if (id != product.Id) return NotFound();

            ModelState.Remove("Slug");
            ModelState.Remove("NameEn");

            // Convert checkbox values "on" to bool
            var sizes = new List<ProductSize>();
            for (int i = 0; i < SizeName.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(SizeName[i]))
                {
                    sizes.Add(new ProductSize
                    {
                        SizeName = SizeName[i],
                        Price = SizePrice[i],
                        IsAvailable = SizeAvailable.Count > i && SizeAvailable[i] == "on",
                        DisplayOrder = i
                    });
                }
            }

            var (success, error) = await _productService.UpdateAsync(product, newImages, sizes);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "حدث خطأ أثناء الحفظ");
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(product);
            }

            TempData["Success"] = "تم تحديث المنتج بنجاح";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _productService.DeleteAsync(id);
            TempData[success ? "Success" : "Error"] = success ? "تم حذف المنتج بنجاح" : "تعذر حذف المنتج";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAvailability(int id)
        {
            await _productService.ToggleAvailabilityAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            await _productService.DeleteImageAsync(imageId);
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetMainImage(int imageId, int productId)
        {
            await _productService.SetMainImageAsync(productId, imageId);
            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        // Moves product images still stored on the server (/images/products/...) to Cloudinary, one batch per click.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveImagesToCloudinary()
        {
            var result = await _productService.MoveLocalImagesToCloudinaryAsync();

            if (!result.CloudinaryEnabled)
            {
                TempData["Error"] = "Cloudinary غير مفعّل على السيرفر. ضيف مفاتيح Cloudinary في appsettings.Production.json الأول.";
            }
            else if (result.Failed.Count > 0)
            {
                TempData["Error"] = $"تم نقل {result.Moved} صورة، وفشل نقل {result.Failed.Count}: {string.Join(" | ", result.Failed)}";
            }
            else if (result.Moved == 0)
            {
                TempData["Success"] = "كل صور المنتجات موجودة على Cloudinary بالفعل";
            }
            else
            {
                TempData["Success"] = result.Remaining > 0
                    ? $"تم نقل {result.Moved} صورة إلى Cloudinary. فاضل {result.Remaining} صورة، دوس على الزرار تاني لنقلها."
                    : $"تم نقل {result.Moved} صورة إلى Cloudinary بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private static List<string> ParseSizesCsv(string? sizesCsv)
        {
            if (string.IsNullOrWhiteSpace(sizesCsv))
                return new List<string>();

            return sizesCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct()
                .ToList();
        }
    }
}
