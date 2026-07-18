using Microsoft.AspNetCore.Identity;

namespace AgriCompany.Web.Models
{
    /// <summary>
    /// Admin user identity. There is NO public registration - admin users are seeded/created by existing admins only.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
