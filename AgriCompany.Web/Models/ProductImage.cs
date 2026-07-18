using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    public class ProductImage
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Required]
        [StringLength(300)]
        public string ImagePath { get; set; } = string.Empty;

        public bool IsMain { get; set; } = false;

        public int DisplayOrder { get; set; } = 0;
    }
}
