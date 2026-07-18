using AgriCompany.Web.Models;

namespace AgriCompany.Web.Services
{
    public interface ICompanyInfoService
    {
        Task<CompanyInformation> GetAsync();
        Task SaveAsync(CompanyInformation info, IFormFile? logo);
    }
}
