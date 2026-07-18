namespace AgriCompany.Web.Models.ViewModels
{
    /// <summary>
    /// Lightweight, dependency-free pagination wrapper used across public listing pages and admin grids.
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }

        public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public static PagedResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize, int totalItems)
        {
            return new PagedResult<T>
            {
                Items = source.ToList(),
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize < 1 ? 12 : pageSize,
                TotalItems = totalItems
            };
        }
    }
}
