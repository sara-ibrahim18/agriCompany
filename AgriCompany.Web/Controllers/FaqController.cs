using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class FaqController : Controller
    {
        private readonly IFaqService _faqService;

        public FaqController(IFaqService faqService)
        {
            _faqService = faqService;
        }

        public async Task<IActionResult> Index()
        {
            var faqs = await _faqService.GetActiveAsync();
            return View(faqs);
        }
    }
}
