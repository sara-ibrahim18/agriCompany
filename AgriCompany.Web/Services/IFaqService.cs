using AgriCompany.Web.Models;

namespace AgriCompany.Web.Services
{
    public interface IFaqService
    {
        Task<List<FAQ>> GetActiveAsync();
        Task<List<FAQ>> GetAllAsync();
        Task<FAQ?> GetByIdAsync(int id);
        Task CreateAsync(FAQ faq);
        Task<bool> UpdateAsync(FAQ faq);
        Task<bool> DeleteAsync(int id);
    }
}
