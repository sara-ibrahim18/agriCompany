using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await _dashboardService.GetDashboardDataAsync();
            return View(vm);
        }
    }
}
