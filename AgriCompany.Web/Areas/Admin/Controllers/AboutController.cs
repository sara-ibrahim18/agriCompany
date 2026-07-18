//using AgriCompany.Web.Models;
//using AgriCompany.Web.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AgriCompany.Web.Areas.Admin.Controllers
//{
//    public class AboutController : AdminBaseController
//    {
//        private readonly ICompanyInfoService _companyInfoService;

//        public AboutController(ICompanyInfoService companyInfoService)
//        {
//            _companyInfoService = companyInfoService;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var info = await _companyInfoService.GetAsync();
//            return View(info);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Index(CompanyInformation info, IFormFile? logo)
//        {
//            if (!ModelState.IsValid)
//                return View(info);

//            await _companyInfoService.SaveAsync(info, logo);
//            TempData["Success"] = "تم تحديث بيانات الشركة بنجاح";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class AboutController : AdminBaseController
    {
        private readonly ICompanyInfoService _companyInfoService;

        public AboutController(ICompanyInfoService companyInfoService)
        {
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> Index()
        {
            var info = await _companyInfoService.GetAsync();
            return View(info);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CompanyInformation info, IFormFile? logo)
        {
            if (!ModelState.IsValid)
                return View(info);

            try
            {
                await _companyInfoService.SaveAsync(info, logo);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(info);
            }

            TempData["Success"] = "تم تحديث بيانات الشركة بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}