using AgriCompany.Web.Data;
using AgriCompany.Web.Models;
using AgriCompany.Web.Repositories;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Database ----------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Configure it in appsettings.json.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ---------- Identity (Admin authentication only - no public registration) ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin/Account/Login";
    options.LogoutPath = "/Admin/Account/Logout";
    options.AccessDeniedPath = "/Admin/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
    options.Cookie.Name = "AgriCompany.Admin.Auth";
});

// ---------- MVC ----------
builder.Services.AddControllersWithViews();

// ---------- Repository / Unit of Work ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ---------- Application Services ----------
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<ISlugService, SlugService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICompanyInfoService, CompanyInfoService>();
//builder.Services.AddScoped<ITestimonialService, TestimonialService>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<IContactMessageService, ContactMessageService>();
builder.Services.AddScoped<IWebsiteSettingService, WebsiteSettingService>();
builder.Services.AddScoped<IHomepageService, HomepageService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var app = builder.Build();

// ---------- Seed database (migrate + seed admin user + starter content) ----------
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider, app.Configuration);
}

// ---------- Middleware pipeline ----------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/NotFound");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin area route (must be registered before the default route)
//app.MapControllerRoute(
//    name: "admin",
//    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
//    defaults: new { area = "Admin" });

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapControllerRoute(
//    name: "admin",
//    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}",
//    defaults: new { area = "Admin" });

app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
