using AgriCompany.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AgriCompany.Web.Data
{
    /// <summary>
    /// Seeds the database with the initial admin user and starter content so the site is usable
    /// immediately after the first migration. Safe to run multiple times (idempotent checks).
    /// </summary>
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            await SeedRolesAndAdminAsync(services, configuration);
            await SeedContentAsync(context);
        }

        private static async Task SeedRolesAndAdminAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var adminEmail = configuration["AdminUser:Email"] ?? "admin@agricompany.com";
            var adminPassword = configuration["AdminUser:Password"] ?? "Admin@12345";

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "مدير النظام",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, adminRole);
                }
            }
        }

        private static async Task SeedContentAsync(ApplicationDbContext context)
        {
            if (!await context.WebsiteSettings.AnyAsync())
            {
                context.WebsiteSettings.Add(new WebsiteSetting
                {
                    SiteTitle = "شركة الأرض الخضراء للمنتجات الزراعية",
                    MetaDescription = "شركة رائدة في توريد الأسمدة والمبيدات والبذور ومستلزمات الزراعة عالية الجودة.",
                    MetaKeywords = "أسمدة، مبيدات، بذور، زراعة، منتجات زراعية",
                    PrimaryColor = "#2e7d32",
                    FooterText = "جميع الحقوق محفوظة",
                    CurrencySymbol = "ج.م",
                    IsMaintenanceMode = false
                });
            }

            if (!await context.CompanyInformation.AnyAsync())
            {
                context.CompanyInformation.Add(new CompanyInformation
                {
                    CompanyNameAr = "شركة الأرض الخضراء للمنتجات الزراعية",
                    CompanyNameEn = "Green Land Agricultural Products Co.",
                    AboutText = "نحن شركة متخصصة في توريد وتصنيع المنتجات الزراعية بجودة عالية منذ أكثر من 15 عامًا، نخدم آلاف المزارعين في جميع أنحاء الجمهورية.",
                    Vision = "أن نكون الخيار الأول للمزارعين في مصر والمنطقة العربية.",
                    Mission = "توفير منتجات زراعية عالية الجودة بأسعار مناسبة مع تقديم الدعم الفني المستمر.",
                    Address = "المنطقة الصناعية، طريق مصر إسماعيلية الصحراوي، القاهرة، مصر",
                    Phone = "01000000000",
                    WhatsAppNumber = "201000000000",
                    Email = "info@agricompany.com",
                    FacebookUrl = "https://facebook.com",
                    InstagramUrl = "https://instagram.com",
                    WorkingHours = "من السبت إلى الخميس: 9 صباحًا - 6 مساءً",
                    GoogleMapsEmbedUrl = "https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3453.006!2d31.2357!3d30.0444!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x0%3A0x0!2zMzDCsDAyJzM5LjgiTiAzMcKwMTQnMDguNiJF!5e0!3m2!1sar!2seg!4v1234567890"
                });
            }

            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { NameAr = "الأسمدة الزراعية", NameEn = "Fertilizers", Slug = "fertilizers", Description = "أسمدة عضوية وكيماوية لجميع المحاصيل", DisplayOrder = 1 },
                    new() { NameAr = "المبيدات", NameEn = "Pesticides", Slug = "pesticides", Description = "مبيدات حشرية وفطرية وعشبية آمنة وفعالة", DisplayOrder = 2 },
                    new() { NameAr = "البذور", NameEn = "Seeds", Slug = "seeds", Description = "بذور عالية الإنتاجية ومقاومة للأمراض", DisplayOrder = 3 },
                    new() { NameAr = "مستلزمات الري", NameEn = "Irrigation Supplies", Slug = "irrigation-supplies", Description = "أنظمة ومعدات الري الحديث", DisplayOrder = 4 },
                    new() { NameAr = "أدوات ومعدات زراعية", NameEn = "Agricultural Tools", Slug = "agricultural-tools", Description = "أدوات ومعدات لخدمة الأرض الزراعية", DisplayOrder = 5 }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();

                var fertilizers = categories.First(c => c.Slug == "fertilizers");
                var pesticides = categories.First(c => c.Slug == "pesticides");
                var seeds = categories.First(c => c.Slug == "seeds");

                var products = new List<Product>
                {
                    new()
                    {
                        NameAr = "سماد نيتروجيني مركز",
                        NameEn = "Concentrated Nitrogen Fertilizer",
                        Code = "FRT-001",
                        Slug = "concentrated-nitrogen-fertilizer",
                        Description = "سماد نيتروجيني عالي التركيز يساعد على نمو المحاصيل بشكل سريع وصحي.",
                        Benefits = "يزيد من كثافة المحصول\nيحسن من لون الأوراق\nسريع الامتصاص",
                        UsageInstructions = "يستخدم بمعدل 25 كجم للفدان، يضاف مع مياه الري.",
                        //Price = 350,
                        //Unit = "كيس 25 كجم",
                        CategoryId = fertilizers.Id,
                        IsAvailable = true,
                        IsFeatured = true,
                        DisplayOrder = 1
                    },
                    new()
                    {
                        NameAr = "سماد عضوي كمبوست",
                        NameEn = "Organic Compost Fertilizer",
                        Code = "FRT-002",
                        Slug = "organic-compost-fertilizer",
                        Description = "سماد عضوي طبيعي 100% يحسن من خصوبة التربة.",
                        Benefits = "يحسن بنية التربة\nصديق للبيئة\nيزيد من احتفاظ التربة بالماء",
                        UsageInstructions = "يستخدم بمعدل 3 أطنان للفدان قبل الزراعة.",
                        //Price = 180,
                        //Unit = "كيس 50 كجم",
                        CategoryId = fertilizers.Id,
                        IsAvailable = true,
                        IsFeatured = false,
                        DisplayOrder = 2
                    },
                    new()
                    {
                        NameAr = "مبيد حشري شامل",
                        NameEn = "Broad Spectrum Insecticide",
                        Code = "PST-001",
                        Slug = "broad-spectrum-insecticide",
                        Description = "مبيد حشري فعال ضد معظم الآفات الزراعية الشائعة.",
                        Benefits = "فعال ضد أكثر من 20 نوع حشرة\nآمن على المحصول\nمفعول طويل المدى",
                        UsageInstructions = "يستخدم بمعدل 100 مل لكل 100 لتر ماء رشًا على الأوراق.",
                        //Price = 220,
                        //Unit = "عبوة 1 لتر",
                        CategoryId = pesticides.Id,
                        IsAvailable = true,
                        IsFeatured = true,
                        DisplayOrder = 1
                    },
                    new()
                    {
                        NameAr = "مبيد فطري وقائي",
                        NameEn = "Preventive Fungicide",
                        Code = "PST-002",
                        Slug = "preventive-fungicide",
                        Description = "مبيد فطري وقائي وعلاجي لحماية المحاصيل من الأمراض الفطرية.",
                        Benefits = "يمنع انتشار الفطريات\nيحمي المحصول من التعفن\nآمن للاستخدام الزراعي",
                        UsageInstructions = "يستخدم بمعدل 250 جم لكل 100 لتر ماء.",
                        //Price = 275,
                        //Unit = "عبوة 500 جم",
                        CategoryId = pesticides.Id,
                        IsAvailable = true,
                        IsFeatured = false,
                        DisplayOrder = 2
                    },
                    new()
                    {
                        NameAr = "بذور طماطم هجين",
                        NameEn = "Hybrid Tomato Seeds",
                        Code = "SED-001",
                        Slug = "hybrid-tomato-seeds",
                        Description = "بذور طماطم هجين عالية الإنتاجية ومقاومة للأمراض.",
                        Benefits = "إنتاجية عالية\nمقاومة للذبول\nثمار متجانسة الحجم",
                        UsageInstructions = "تزرع في مشتل قبل النقل بـ 30 يومًا.",
                        //Price = 450,
                        //Unit = "علبة 10 جم",
                        CategoryId = seeds.Id,
                        IsAvailable = true,
                        IsFeatured = true,
                        DisplayOrder = 1
                    },
                    new()
                    {
                        NameAr = "بذور خيار هولندي",
                        NameEn = "Dutch Cucumber Seeds",
                        Code = "SED-002",
                        Slug = "dutch-cucumber-seeds",
                        Description = "بذور خيار هولندي مبكرة النضج وعالية الجودة.",
                        Benefits = "نضج مبكر\nطعم ممتاز\nمقاومة عالية للأمراض",
                        UsageInstructions = "تزرع مباشرة في الأرض بعد تجهيزها.",
                        //Price = 380,
                        //Unit = "علبة 10 جم",
                        CategoryId = seeds.Id,
                        IsAvailable = true,
                        IsFeatured = false,
                        DisplayOrder = 2
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();

                foreach (var product in products)
                {
                    context.ProductSizes.Add(new ProductSize { ProductId = product.Id, SizeName = "صغير", DisplayOrder = 1 });
                    context.ProductSizes.Add(new ProductSize { ProductId = product.Id, SizeName = "متوسط", DisplayOrder = 2 });
                    context.ProductSizes.Add(new ProductSize { ProductId = product.Id, SizeName = "كبير", DisplayOrder = 3 });
                }
                await context.SaveChangesAsync();
            }

            if (!await context.FAQs.AnyAsync())
            {
                context.FAQs.AddRange(
                    new FAQ { QuestionAr = "كيف يمكنني طلب منتج؟", AnswerAr = "يمكنك تصفح المنتجات واختيار المنتج المطلوب ثم الضغط على زر (اطلب عبر واتساب) وسيتم فتح محادثة واتساب مباشرة مع فريق المبيعات.", DisplayOrder = 1 },
                    new FAQ { QuestionAr = "هل يوجد توصيل لجميع المحافظات؟", AnswerAr = "نعم، نقوم بالتوصيل لجميع محافظات الجمهورية عبر شركات الشحن المعتمدة.", DisplayOrder = 2 },
                    new FAQ { QuestionAr = "هل المنتجات أصلية ومضمونة؟", AnswerAr = "جميع منتجاتنا أصلية 100% ومعتمدة من وزارة الزراعة المصرية.", DisplayOrder = 3 },
                    new FAQ { QuestionAr = "ما هي طرق الدفع المتاحة؟", AnswerAr = "يتم تحديد طريقة الدفع عند التواصل معنا عبر واتساب حسب الكمية والمنطقة.", DisplayOrder = 4 }
                );
                await context.SaveChangesAsync();
            }

            if (!await context.HomepageSlides.AnyAsync())
            {
                context.HomepageSlides.AddRange(
                    new HomepageSlide { TitleAr = "منتجات زراعية بجودة عالمية", SubtitleAr = "أسمدة ومبيدات وبذور معتمدة لزيادة إنتاجية أرضك", ImagePath = "/images/slides/slide1.jpg", ButtonText = "تصفح المنتجات", ButtonUrl = "/Products", DisplayOrder = 1 },
                    new HomepageSlide { TitleAr = "دعم فني على مدار الأسبوع", SubtitleAr = "فريق من المهندسين الزراعيين لخدمتك", ImagePath = "/images/slides/slide2.jpg", ButtonText = "تواصل معنا", ButtonUrl = "/Contact", DisplayOrder = 2 }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}