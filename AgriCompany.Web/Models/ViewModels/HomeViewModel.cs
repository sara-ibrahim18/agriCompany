namespace AgriCompany.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<HomepageSlide> Slides { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
        //public List<Testimonial> Testimonials { get; set; } = new();
        public CompanyInformation? Company { get; set; }
    }
}
