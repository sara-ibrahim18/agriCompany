namespace AgriCompany.Web.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ProductsCount { get; set; }
        public int CategoriesCount { get; set; }
        public int AvailableProductsCount { get; set; }
        public int UnavailableProductsCount { get; set; }
        public int NewMessagesCount { get; set; }
        public int TotalMessagesCount { get; set; }
        public int TestimonialsCount { get; set; }
        public int FaqCount { get; set; }
        public List<Product> LatestProducts { get; set; } = new();
        public List<ContactMessage> LatestMessages { get; set; } = new();
        public List<CategoryProductCount> ProductsPerCategory { get; set; } = new();
    }

    public class CategoryProductCount
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
