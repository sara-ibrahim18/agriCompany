using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriCompany.Web.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المنتج بالعربية مطلوب")]
        [StringLength(200)]
        [Display(Name = "الاسم بالعربية")]
        public string NameAr { get; set; } = string.Empty;

        //[Required(ErrorMessage = "اسم المنتج بالإنجليزية مطلوب")]
        //[StringLength(200)]
        //[Display(Name = "الاسم بالإنجليزية")]
        //public string NameEn { get; set; } = string.Empty;
        [StringLength(200)]
        [Display(Name = "الاسم بالإنجليزية")]
        public string? NameEn { get; set; } = null!;

        //[Required(ErrorMessage = "كود المنتج مطلوب")]
        //[StringLength(50)]
        //[Display(Name = "كود المنتج")]
        //public string Code { get; set; } = string.Empty;
        // الكود بيتولد تلقائيًا من السيرفر، مش بيتكتب يدويًا
        [StringLength(50)]
        [Display(Name = "كود المنتج")]
        public string Code { get; set; } = string.Empty;

        //[Required]
        //[StringLength(220)]
        //public string Slug { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false)]
        [StringLength(220)]
        public string Slug { get; set; } = string.Empty;
        [Required(ErrorMessage = "الوصف مطلوب")]
        [Display(Name = "الوصف")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "الفوائد")]
        public string? Benefits { get; set; }

        [Display(Name = "طريقة الاستخدام")]
        public string? UsageInstructions { get; set; }

        //[Required(ErrorMessage = "السعر مطلوب")]
        //[Column(TypeName = "decimal(18,2)")]
        //[Display(Name = "السعر")]
        //[Range(0, 9999999, ErrorMessage = "السعر يجب أن يكون رقمًا موجبًا")]
        //public decimal Price { get; set; }

        //[Display(Name = "الوحدة")]
        //[StringLength(50)]
        //public string? Unit { get; set; }

        [Required]
        [Display(Name = "القسم")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Display(Name = "متاح")]
        public bool IsAvailable { get; set; } = true;

        [Display(Name = "منتج مميز")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "ترتيب العرض")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "عدد المشاهدات")]
        public int ViewsCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public ICollection<ProductSize> Sizes { get; set; } = new List<ProductSize>();

        [NotMapped]
        public string? MainImagePath => Images?.OrderByDescending(i => i.IsMain).ThenBy(i => i.DisplayOrder).FirstOrDefault()?.ImagePath;
    }
}
