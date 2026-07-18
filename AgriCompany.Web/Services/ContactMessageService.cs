using AgriCompany.Web.Models;
using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactMessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SubmitAsync(ContactMessage message)
        {
            message.CreatedAt = DateTime.Now;
            message.IsRead = false;
            await _unitOfWork.ContactMessages.AddAsync(message);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<PagedResult<ContactMessage>> GetPagedAsync(int pageNumber, bool? unreadOnly, int pageSize = 15)
        {
            var query = _unitOfWork.ContactMessages.Query();

            if (unreadOnly == true)
                query = query.Where(m => !m.IsRead);

            query = query.OrderByDescending(m => m.CreatedAt);

            var totalItems = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return PagedResult<ContactMessage>.Create(items, pageNumber, pageSize, totalItems);
        }

        public async Task<ContactMessage?> GetByIdAsync(int id, bool markAsRead = false)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
            if (message != null && markAsRead && !message.IsRead)
            {
                message.IsRead = true;
                _unitOfWork.ContactMessages.Update(message);
                await _unitOfWork.CompleteAsync();
            }
            return message;
        }

        public async Task<int> GetUnreadCountAsync() => await _unitOfWork.ContactMessages.CountAsync(m => !m.IsRead);

        public async Task<bool> DeleteAsync(int id)
        {
            var message = await _unitOfWork.ContactMessages.GetByIdAsync(id);
            if (message == null) return false;

            _unitOfWork.ContactMessages.Remove(message);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
