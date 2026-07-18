using AgriCompany.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductSize> ProductSizes { get; set; } = null!;
        public DbSet<CompanyInformation> CompanyInformation { get; set; } = null!;
        //public DbSet<Testimonial> Testimonials { get; set; } = null!;
        public DbSet<FAQ> FAQs { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<WebsiteSetting> WebsiteSettings { get; set; } = null!;
        public DbSet<HomepageSlide> HomepageSlides { get; set; } = null!;
        public DbSet<ProductSlugHistory> ProductSlugHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
                entity.Property(c => c.NameAr).IsRequired();
            });

            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Slug).IsUnique();
                entity.HasIndex(p => p.Code).IsUnique();

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(pi => pi.Product)
                      .WithMany(p => p.Images)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductSize>(entity =>
            {
                entity.HasOne(ps => ps.Product)
                      .WithMany(p => p.Sizes)
                      .HasForeignKey(ps => ps.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
