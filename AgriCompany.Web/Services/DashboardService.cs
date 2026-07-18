using AgriCompany.Web.Models.ViewModels;
using AgriCompany.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var vm = new DashboardViewModel
            {
                ProductsCount = await _unitOfWork.Products.CountAsync(),
                CategoriesCount = await _unitOfWork.Categories.CountAsync(),
                AvailableProductsCount = await _unitOfWork.Products.CountAsync(p => p.IsAvailable),
                UnavailableProductsCount = await _unitOfWork.Products.CountAsync(p => !p.IsAvailable),
                NewMessagesCount = await _unitOfWork.ContactMessages.CountAsync(m => !m.IsRead),
                TotalMessagesCount = await _unitOfWork.ContactMessages.CountAsync(),
                //TestimonialsCount = await _unitOfWork.Testimonials.CountAsync(),
                FaqCount = await _unitOfWork.FAQs.CountAsync(),
                LatestProducts = await _unitOfWork.Products.QueryWithDetails()
                    .OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync(),
                LatestMessages = await _unitOfWork.ContactMessages.Query()
                    .OrderByDescending(m => m.CreatedAt).Take(5).ToListAsync()
            };

            vm.ProductsPerCategory = await _unitOfWork.Products.QueryWithDetails()
                .GroupBy(p => p.Category!.NameAr)
                .Select(g => new CategoryProductCount { CategoryName = g.Key, Count = g.Count() })
                .ToListAsync();

            return vm;
        }
    }
}
