using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    /// <summary>
    /// Homepage hero/banner slides managed by admin (Swiper.js slider on the public homepage).
    /// </summary>
    public class HomepageSlide
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "العنوان")]
        public string TitleAr { get; set; } = string.Empty;

        [StringLength(400)]
        [Display(Name = "العنوان الفرعي")]
        public string? SubtitleAr { get; set; }

        [Required, StringLength(300)]
        [Display(Name = "الصورة")]
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "نص الزر")]
        public string? ButtonText { get; set; }

        [StringLength(300)]
        [Display(Name = "رابط الزر")]
        public string? ButtonUrl { get; set; }

        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ترتيب العرض")]
        public int DisplayOrder { get; set; } = 0;
    }
}
