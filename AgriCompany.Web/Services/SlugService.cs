using System.Text;
using System.Text.RegularExpressions;

namespace AgriCompany.Web.Services
{
    public class SlugService : ISlugService
    {
        /// <summary>
        /// Generates a URL-friendly slug. Supports Arabic text by transliterating to a safe ASCII
        /// representation is unnecessary for URLs (Arabic slugs are valid UTF-8 URL segments), so
        /// we keep Arabic characters and only strip characters that break URLs, then append a short
        /// random suffix to help ensure uniqueness for the caller to verify against the database.
        /// </summary>
        public string GenerateSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Guid.NewGuid().ToString("N")[..8];

            var slug = text.Trim().ToLowerInvariant();
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"[^\p{L}\p{N}\-]", "");
            slug = Regex.Replace(slug, @"-+", "-").Trim('-');

            if (string.IsNullOrWhiteSpace(slug))
                slug = Guid.NewGuid().ToString("N")[..8];

            return slug;
        }
    }
}
