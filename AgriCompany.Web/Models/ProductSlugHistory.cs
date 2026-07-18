namespace AgriCompany.Web.Models
{
    public class ProductSlugHistory
    {
        public int Id { get; set; }
        public string OldSlug { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}