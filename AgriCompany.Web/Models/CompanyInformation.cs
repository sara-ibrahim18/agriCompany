using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    /// <summary>
    /// Singleton-style table (one active row) holding company profile info shown across the public site.
    /// </summary>
    public class CompanyInformation
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "اسم الشركة بالعربية")]
        public string CompanyNameAr { get; set; } = string.Empty;

        [Required, StringLength(200)]
        [Display(Name = "اسم الشركة بالإنجليزية")]
        public string CompanyNameEn { get; set; } = string.Empty;

        [StringLength(300)]
        [Display(Name = "الشعار")]
        public string? LogoPath { get; set; }

        [Display(Name = "نبذة عن الشركة")]
        public string? AboutText { get; set; }

        [Display(Name = "رؤيتنا")]
        public string? Vision { get; set; }

        [Display(Name = "رسالتنا")]
        public string? Mission { get; set; }

        [StringLength(400)]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Required, StringLength(30)]
        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; } = string.Empty;

        [Required, StringLength(30)]
        [Display(Name = "رقم واتساب (بالصيغة الدولية، مثال: 201234567890)")]
        public string WhatsAppNumber { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [StringLength(300)]
        [Display(Name = "رابط فيسبوك")]
        public string? FacebookUrl { get; set; }

        [StringLength(300)]
        [Display(Name = "رابط انستجرام")]
        public string? InstagramUrl { get; set; }

        [StringLength(300)]
        [Display(Name = "رابط تويتر / X")]
        public string? TwitterUrl { get; set; }

        [StringLength(300)]
        [Display(Name = "رابط يوتيوب")]
        public string? YoutubeUrl { get; set; }

        [Display(Name = "كود خرائط جوجل (Embed)")]
        public string? GoogleMapsEmbedUrl { get; set; }

        [StringLength(300)]
        [Display(Name = "مواعيد العمل")]
        public string? WorkingHours { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
