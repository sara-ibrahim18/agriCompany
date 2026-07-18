using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    /// <summary>
    /// Singleton-style table (one active row) for general SEO/site-wide settings.
    /// </summary>
    public class WebsiteSetting
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "عنوان الموقع")]
        public string SiteTitle { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "وصف الموقع (Meta Description)")]
        public string? MetaDescription { get; set; }

        [StringLength(500)]
        [Display(Name = "كلمات مفتاحية (Meta Keywords)")]
        public string? MetaKeywords { get; set; }

        [StringLength(20)]
        [Display(Name = "اللون الأساسي")]
        public string PrimaryColor { get; set; } = "#2e7d32";

        [Display(Name = "نص الفوتر")]
        public string? FooterText { get; set; }

        [StringLength(10)]
        [Display(Name = "رمز العملة")]
        public string CurrencySymbol { get; set; } = "ج.م";

        [Display(Name = "تفعيل الموقع (وضع الصيانة)")]
        public bool IsMaintenanceMode { get; set; } = false;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
