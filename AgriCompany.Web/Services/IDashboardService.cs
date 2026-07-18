using AgriCompany.Web.Models.ViewModels;

namespace AgriCompany.Web.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
