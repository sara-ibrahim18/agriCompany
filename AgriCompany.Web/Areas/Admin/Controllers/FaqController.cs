using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class FaqController : AdminBaseController
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await _faqService.GetAllAsync();
            return View(faqs);
        }

        public IActionResult Create() => View(new FAQ());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FAQ faq)
        {
            if (!ModelState.IsValid) return View(faq);

            await _faqService.CreateAsync(faq);
            TempData["Success"] = "تم إضافة السؤال بنجاح";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var faq = await _faqService.GetByIdAsync(id);
            if (faq == null) return NotFound();
            return View(faq);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FAQ faq)
        {
            if (id != faq.Id) return NotFound();
            if (!ModelState.IsValid) return View(faq);

            await _faqService.UpdateAsync(faq);
            TempData["Success"] = "تم تحديث السؤال بنجاح";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _faqService.DeleteAsync(id);
            TempData["Success"] = "تم حذف السؤال بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
