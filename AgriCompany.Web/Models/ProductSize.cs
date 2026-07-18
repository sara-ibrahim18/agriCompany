using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriCompany.Web.Models
{
    public class ProductSize
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Required]
        [Display(Name = "اسم الحجم")]
        public string SizeName { get; set; } = string.Empty;   // صغير، 25 كجم، كبير...

        [Required]
        [Display(Name = "السعر")]
        [Column(TypeName = "decimal(18,2)")]   // ← أضف ده
        public decimal Price { get; set; }

        [Display(Name = "ترتيب العرض")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "متاح")]
        public bool IsAvailable { get; set; } = true;
    }
    //public class ProductSize
    //{
    //    public int Id { get; set; }

    //    [Required]
    //    public int ProductId { get; set; }
    //    public Product? Product { get; set; }

    //    [Required(ErrorMessage = "اسم الحجم مطلوب")]
    //    [StringLength(100)]
    //    [Display(Name = "الحجم")]
    //    public string SizeName { get; set; } = string.Empty; // e.g. "1 كجم", "5 لتر", "علبة صغيرة"
    //    public decimal Price { get; set; }
    //    public bool IsAvailable { get; set; } = true;

    //    public int DisplayOrder { get; set; } = 0;
    //}
}
