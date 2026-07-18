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
