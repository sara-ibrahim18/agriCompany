using AgriCompany.Web.Models;
using AgriCompany.Web.Models.ViewModels;

namespace AgriCompany.Web.Services
{
    public interface IContactMessageService
    {
        Task SubmitAsync(ContactMessage message);
        Task<PagedResult<ContactMessage>> GetPagedAsync(int pageNumber, bool? unreadOnly, int pageSize = 15);
        Task<ContactMessage?> GetByIdAsync(int id, bool markAsRead = false);
        Task<int> GetUnreadCountAsync();
        Task<bool> DeleteAsync(int id);
    }
}
