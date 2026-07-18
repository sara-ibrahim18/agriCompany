using AgriCompany.Web.Models;

namespace AgriCompany.Web.Repositories
{
    /// <summary>
    /// Coordinates repositories under a single DbContext/transaction scope.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IRepository<ProductImage> ProductImages { get; }
        IRepository<ProductSize> ProductSizes { get; }
        IRepository<Models.CompanyInformation> CompanyInformation { get; }
        //IRepository<Models.Testimonial> Testimonials { get; }
        IRepository<Models.FAQ> FAQs { get; }
        IRepository<Models.ContactMessage> ContactMessages { get; }
        IRepository<Models.WebsiteSetting> WebsiteSettings { get; }
        IRepository<Models.HomepageSlide> HomepageSlides { get; }
        IRepository<Models.ProductSlugHistory> ProductSlugHistories { get; }

        Task<int> CompleteAsync();
    }
}
