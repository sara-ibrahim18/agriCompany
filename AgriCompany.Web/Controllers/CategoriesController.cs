using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetActiveAsync();
            return View(categories);
        }
    }
}
