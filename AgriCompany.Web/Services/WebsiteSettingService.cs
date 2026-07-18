using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;

namespace AgriCompany.Web.Services
{
    public class WebsiteSettingService : IWebsiteSettingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WebsiteSettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WebsiteSetting> GetAsync()
        {
            var setting = (await _unitOfWork.WebsiteSettings.GetAllAsync()).FirstOrDefault();
            if (setting == null)
            {
                setting = new WebsiteSetting { SiteTitle = "الموقع الزراعي" };
                await _unitOfWork.WebsiteSettings.AddAsync(setting);
                await _unitOfWork.CompleteAsync();
            }
            return setting;
        }

        public async Task SaveAsync(WebsiteSetting setting)
        {
            var existing = (await _unitOfWork.WebsiteSettings.GetAllAsync()).FirstOrDefault();

            if (existing == null)
            {
                // أول مرة
                setting.UpdatedAt = DateTime.Now;
                await _unitOfWork.WebsiteSettings.AddAsync(setting);
            }
            else
            {
                // تحديث
                existing.SiteTitle = setting.SiteTitle;
                existing.MetaDescription = setting.MetaDescription;
                existing.MetaKeywords = setting.MetaKeywords;
                existing.PrimaryColor = setting.PrimaryColor;
                existing.CurrencySymbol = setting.CurrencySymbol;
                existing.FooterText = setting.FooterText;
                existing.IsMaintenanceMode = setting.IsMaintenanceMode;
                existing.UpdatedAt = DateTime.Now;

                _unitOfWork.WebsiteSettings.Update(existing);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}
