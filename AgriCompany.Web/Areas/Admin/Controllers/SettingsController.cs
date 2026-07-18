using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class SettingsController : AdminBaseController
    {
        private readonly IWebsiteSettingService _websiteSettingService;

        public SettingsController(IWebsiteSettingService websiteSettingService)
        {
            _websiteSettingService = websiteSettingService;
        }

        public async Task<IActionResult> Index()
        {
            var setting = await _websiteSettingService.GetAsync();
            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(WebsiteSetting setting)
        {
            if (!ModelState.IsValid) return View(setting);

            await _websiteSettingService.SaveAsync(setting);
            TempData["Success"] = "تم حفظ الإعدادات بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
