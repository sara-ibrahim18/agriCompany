using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var paged = await _categoryService.GetPagedAsync(page);
            return View(paged);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return View(category);

            var (success, error) = await _categoryService.CreateAsync(category, image);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "حدث خطأ أثناء الحفظ");
                return View(category);
            }

            TempData["Success"] = "تم إضافة القسم بنجاح";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category, IFormFile? image)
        {
            if (id != category.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(category);

            var (success, error) = await _categoryService.UpdateAsync(category, image);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "حدث خطأ أثناء الحفظ");
                return View(category);
            }

            TempData["Success"] = "تم تحديث القسم بنجاح";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _categoryService.DeleteAsync(id);
            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "تم حذف القسم بنجاح";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
