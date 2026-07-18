using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class AboutController : Controller
    {
        private readonly ICompanyInfoService _companyInfoService;

        public AboutController(ICompanyInfoService companyInfoService)
        {
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> Index()
        {
            var company = await _companyInfoService.GetAsync();
            return View(company);
        }
    }
}
