using System.Text;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public SitemapController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> Index()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

            void AddUrl(string path, string changefreq, string priority)
            {
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{baseUrl}{path}</loc>");
                sb.AppendLine($"    <lastmod>{today}</lastmod>");
                sb.AppendLine($"    <changefreq>{changefreq}</changefreq>");
                sb.AppendLine($"    <priority>{priority}</priority>");
                sb.AppendLine("  </url>");
            }

            AddUrl("/", "daily", "1.0");
            AddUrl("/About", "monthly", "0.6");
            AddUrl("/Contact", "monthly", "0.5");
            AddUrl("/Faq", "monthly", "0.5");
            AddUrl("/Categories", "weekly", "0.7");
            AddUrl("/Products", "daily", "0.9");

            var categories = await _categoryService.GetActiveAsync();
            foreach (var cat in categories)
                AddUrl($"/Products/ByCategory/{cat.Slug}", "weekly", "0.7");

            var products = await _productService.GetPublicProductsAsync(null, null, "newest", onlyAvailable: false, pageNumber: 1, pageSize: int.MaxValue);
            foreach (var product in products.Items)
                AddUrl($"/Products/Details/{product.Slug}", "weekly", "0.8");

            sb.AppendLine("</urlset>");
            return Content(sb.ToString(), "application/xml", Encoding.UTF8);
        }
    }
}