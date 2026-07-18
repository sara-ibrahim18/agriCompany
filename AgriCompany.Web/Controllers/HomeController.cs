using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomepageService _homepageService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        //private readonly ITestimonialService _testimonialService;
        private readonly ICompanyInfoService _companyInfoService;

        public HomeController(
            IHomepageService homepageService,
            ICategoryService categoryService,
            IProductService productService,
            //ITestimonialService testimonialService,
            ICompanyInfoService companyInfoService)
        {
            _homepageService = homepageService;
            _categoryService = categoryService;
            _productService = productService;
            //_testimonialService = testimonialService;
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                Slides = await _homepageService.GetActiveSlidesAsync(),
                Categories = await _categoryService.GetActiveAsync(),
                FeaturedProducts = await _productService.GetFeaturedAsync(8),
                //Testimonials = await _testimonialService.GetActiveAsync(),
                Company = await _companyInfoService.GetAsync()
            };

            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
        [Route("/Home/NotFound")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
