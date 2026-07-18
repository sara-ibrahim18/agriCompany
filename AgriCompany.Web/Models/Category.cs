using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم بالعربي مطلوب")]
        [StringLength(150)]
        [Display(Name = "الاسم بالعربية")]
        public string NameAr { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم بالإنجليزي مطلوب")]
        [StringLength(150)]
        [Display(Name = "الاسم بالإنجليزية")]
        public string NameEn { get; set; } = string.Empty;

   
        [StringLength(170)]
        public string Slug { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [StringLength(300)]
        [Display(Name = "صورة القسم")]
        public string? ImagePath { get; set; }

        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ترتيب العرض")]
        public int DisplayOrder { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
