using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly ICompanyInfoService _companyInfoService;
        private readonly IContactMessageService _contactMessageService;

        public ContactController(ICompanyInfoService companyInfoService, IContactMessageService contactMessageService)
        {
            _companyInfoService = companyInfoService;
            _contactMessageService = contactMessageService;
        }

        public async Task<IActionResult> Index()
        {
            var company = await _companyInfoService.GetAsync();
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactMessage message)
        {
            if (!ModelState.IsValid)
            {
                var company = await _companyInfoService.GetAsync();
                ViewBag.CompanyInfo = company;
                TempData["ContactError"] = "من فضلك تأكد من تعبئة جميع الحقول المطلوبة بشكل صحيح.";
                return View(company);
            }

            await _contactMessageService.SubmitAsync(message);
            TempData["ContactSuccess"] = "تم إرسال رسالتك بنجاح، سيتم التواصل معك في أقرب وقت.";
            return RedirectToAction(nameof(Index));
        }
    }
}
