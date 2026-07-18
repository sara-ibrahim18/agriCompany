using AgriCompany.Web.Models;

namespace AgriCompany.Web.Services
{
    public interface IWebsiteSettingService
    {
        Task<WebsiteSetting> GetAsync();
        Task SaveAsync(WebsiteSetting setting);
    }
}
