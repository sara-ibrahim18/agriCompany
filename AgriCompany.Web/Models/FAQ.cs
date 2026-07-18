using System.ComponentModel.DataAnnotations;

namespace AgriCompany.Web.Models
{
    public class FAQ
    {
        public int Id { get; set; }

        [Required, StringLength(500)]
        [Display(Name = "السؤال")]
        public string QuestionAr { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الإجابة")]
        public string AnswerAr { get; set; } = string.Empty;

        [Display(Name = "مفعل")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "ترتيب العرض")]
        public int DisplayOrder { get; set; } = 0;
    }
}
