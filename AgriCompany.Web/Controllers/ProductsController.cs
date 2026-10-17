using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: /Products
        public async Task<IActionResult> Index(string? search, int? categoryId, string sortBy = "newest", int page = 1)
        {
            var pagedProducts = await _productService.GetPublicProductsAsync(search, categoryId, sortBy, onlyAvailable: false, pageNumber: page);

            var vm = new ProductListViewModel
            {
                Products = pagedProducts,
                Categories = await _categoryService.GetActiveAsync(),
                SearchTerm = search,
                CategoryId = categoryId,
                SortBy = sortBy,
                PageNumber = page
            };

            return View(vm);
        }

        // GET: /Products/Details/{slug}
        //[Route("Products/Details/{slug}")]
        //public async Task<IActionResult> Details(string slug)
        //{
        //    if (string.IsNullOrWhiteSpace(slug))
        //        return NotFound();

        //    var product = await _productService.GetBySlugAsync(slug);
        //    if (product == null)
        //        return NotFound();

        //    ViewBag.RelatedProducts = await _productService.GetRelatedAsync(product.Id, product.CategoryId);
        //    return View(product);
        //}
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var product = await _productService.GetBySlugAsync(slug);
            if (product == null)
            {
                var newSlug = await _productService.GetCurrentSlugFromOldSlugAsync(slug);
                if (!string.IsNullOrWhiteSpace(newSlug))
                {
                    return RedirectToActionPermanent(nameof(Details), new { slug = newSlug });
                }
                return NotFound();
            }

            ViewBag.RelatedProducts = await _productService.GetRelatedAsync(product.Id, product.CategoryId);
            return View(product);
        }
        // GET: /Products/Suggest?q=...
        // Lightweight JSON endpoint used by the navbar quick-search box to show
        // live suggestions while the user types, so they reach the product
        // they want without a full page reload.
        [HttpGet]
        [Route("Products/Suggest")]
        public async Task<IActionResult> Suggest(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Json(Array.Empty<object>());

            var paged = await _productService.GetPublicProductsAsync(q, null, "newest", onlyAvailable: false, pageNumber: 1, pageSize: 6);

            var result = paged.Items.Select(p => new
            {
                name = p.NameAr,
                slug = p.Slug,
                image = string.IsNullOrWhiteSpace(p.MainImagePath) ? "/images/placeholders/category.jpg" : p.MainImagePath,
                category = p.Category?.NameAr
            });

            return Json(result);
        }

        // GET: /Products/ByCategory/{slug}
        [Route("Products/ByCategory/{slug}")]
        public async Task<IActionResult> ByCategory(string slug, string? search, string sortBy = "newest", int page = 1)
        {
            var category = await _categoryService.GetBySlugAsync(slug);
            if (category == null)
                return NotFound();

            var pagedProducts = await _productService.GetPublicProductsAsync(search, category.Id, sortBy, onlyAvailable: false, pageNumber: page);

            var vm = new ProductListViewModel
            {
                Products = pagedProducts,
                Categories = await _categoryService.GetActiveAsync(),
                SearchTerm = search,
                CategoryId = category.Id,
                SortBy = sortBy,
                PageNumber = page
            };

            ViewBag.CurrentCategory = category;
            return View("Index", vm);
        }
    }
}