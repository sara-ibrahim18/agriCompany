using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب"), StringLength(150)]
        [Display(Name = "الاسم بالكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب"), StringLength(30)]
        [Display(Name = "رقم الهاتف")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "الموضوع مطلوب"), StringLength(200)]
        [Display(Name = "الموضوع")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "نص الرسالة مطلوب")]
        [Display(Name = "الرسالة")]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
