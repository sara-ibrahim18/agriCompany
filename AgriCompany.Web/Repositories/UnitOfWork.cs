using AgriCompany.Web.Data;
using AgriCompany.Web.Models;

namespace AgriCompany.Web.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private IRepository<ProductImage>? _productImages;
        private IRepository<ProductSize>? _productSizes;
        private IRepository<CompanyInformation>? _companyInformation;
        //private IRepository<Testimonial>? _testimonials;
        private IRepository<FAQ>? _faqs;
        private IRepository<ContactMessage>? _contactMessages;
        private IRepository<WebsiteSetting>? _websiteSettings;
        private IRepository<HomepageSlide>? _homepageSlides;
        private IRepository<ProductSlugHistory>? _productSlugHistories;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IRepository<ProductImage> ProductImages => _productImages ??= new Repository<ProductImage>(_context);
        public IRepository<ProductSize> ProductSizes => _productSizes ??= new Repository<ProductSize>(_context);
        public IRepository<CompanyInformation> CompanyInformation => _companyInformation ??= new Repository<CompanyInformation>(_context);
        //public IRepository<Testimonial> Testimonials => _testimonials ??= new Repository<Testimonial>(_context);
        public IRepository<FAQ> FAQs => _faqs ??= new Repository<FAQ>(_context);
        public IRepository<ContactMessage> ContactMessages => _contactMessages ??= new Repository<ContactMessage>(_context);
        public IRepository<WebsiteSetting> WebsiteSettings => _websiteSettings ??= new Repository<WebsiteSetting>(_context);
        public IRepository<HomepageSlide> HomepageSlides => _homepageSlides ??= new Repository<HomepageSlide>(_context);
        public IRepository<ProductSlugHistory> ProductSlugHistories => _productSlugHistories ??= new Repository<ProductSlugHistory>(_context);
        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
