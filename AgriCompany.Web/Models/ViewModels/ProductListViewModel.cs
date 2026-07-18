namespace AgriCompany.Web.Models.ViewModels
{
    /// <summary>
    /// View model backing the public Products listing page (search + filter + sort + pagination).
    /// </summary>
    public class ProductListViewModel
    {
        public PagedResult<Product> Products { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string SortBy { get; set; } = "newest"; // newest, price_asc, price_desc, name_asc
        public bool OnlyAvailable { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
