# AgriCompany.Web — موقع الشركة الزراعية

موقع متكامل (ASP.NET Core MVC .NET 9) لعرض منتجات شركة زراعية، مع طلب عبر واتساب فقط (بدون سلة أو دفع إلكتروني)، ولوحة تحكم كاملة للأدمن.

## ⚠️ ملاحظة مهمة قبل التشغيل

هذا المشروع **لم يتم تجربة بنائه فعليًا بـ `dotnet build`** لأن بيئة الإنشاء لا تحتوي على .NET SDK أو اتصال بـ NuGet. الكود تمت كتابته بعناية بالاعتماد على معرفة دقيقة بـ APIs الخاصة بـ .NET 9 / EF Core 9 / ASP.NET Core Identity، لكن يُنصح بمراجعة أي أخطاء بناء قد تظهر عندك محليًا (غالبًا بسيطة، زي عدم تطابق إصدار حزمة).

## المتطلبات

- **.NET 9 SDK**
- **SQL Server** (LocalDB يكفي للتطوير، أو SQL Server كامل / Express)
- **Visual Studio 2022 (17.8+)** أو **VS Code** أو **Rider**

## خطوات التشغيل

### 1. فك الضغط وفتح المشروع
```
cd AgriCompany.Web
```

### 2. استعادة الحزم (NuGet)
```
dotnet restore
```

### 3. تعديل سلسلة الاتصال (لو لازم)
افتح `appsettings.json` وعدّل `ConnectionStrings:DefaultConnection` لو مش هتستخدم LocalDB الافتراضي:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AgriCompanyDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

### 4. إنشاء أول Migration
المشروع **لا يحتوي على مجلد Migrations بعد** لأنه لازم يتولّد من جهازك (محتاج EF Core Tools مثبتة فعليًا مع SDK حقيقي):
```
dotnet tool install --global dotnet-ef   # لو مش مثبتة قبل كده
dotnet ef migrations add InitialCreate
```

### 5. تشغيل المشروع
```
dotnet run
```
عند أول تشغيل، الكود في `Program.cs` بينفّذ تلقائيًا:
- `Database.MigrateAsync()` → بينشئ قاعدة البيانات فعليًا من الـ Migration
- بيزرع Role باسم `Admin` ومستخدم أدمن افتراضي
- بيزرع بيانات تجريبية (أقسام، منتجات، آراء عملاء، أسئلة شائعة، سلايدات)

### 6. تسجيل الدخول للوحة التحكم
افتح: `https://localhost:{port}/Admin/Account/Login`

بيانات الدخول الافتراضية (موجودة في `appsettings.json` تحت `AdminUser`، **يُنصح بتغييرها فورًا**):
- البريد: `admin@agricompany.com`
- كلمة المرور: `Admin@12345`

## ملاحظات مهمة

- **كل مكتبات الواجهة (Bootstrap, Font Awesome, jQuery, Swiper, AOS, Toastr, SweetAlert2, Lightbox2, خط Cairo) مُستضافة محليًا** جوه `wwwroot/lib`، مفيش أي اعتماد على CDN خارجي — الموقع هيشتغل بالكامل حتى بدون إنترنت.
- **الصور الافتراضية** (سلايدر الرئيسية + صور بديلة للمنتجات/الأقسام/آراء العملاء) اتحطت كملفات فعلية جوه `wwwroot/images`. يُنصح باستبدالها بصور حقيقية من لوحة التحكم بعد أول تشغيل.
- **لا يوجد تسجيل عام للمستخدمين** — فقط تسجيل دخول للأدمن، ولا يظهر أي رابط للوحة التحكم في الموقع العام.
- **الطلبات تتم فقط عبر واتساب** (لا يوجد Cart / Checkout / Orders / Payment في النظام، طبقًا للمواصفات).
- رقم الواتساب المستخدم في كل روابط الطلب مصدره حقل `WhatsAppNumber` في بيانات الشركة (لوحة التحكم → بيانات الشركة)، عدّله من هناك بالصيغة الدولية بدون `+` (مثال: `201001234567`).

## هيكل المشروع (ملخص)

```
AgriCompany.Web/
├── Models/                 # الكيانات (Product, Category, ...)
├── Data/                   # ApplicationDbContext + SeedData
├── Repositories/           # Repository Pattern + Unit of Work
├── Services/                # طبقة الخدمات (Business Logic)
├── Controllers/             # كنترولرز الموقع العام
├── Areas/Admin/              # لوحة التحكم بالكامل (منفصلة تمامًا عن الموقع العام)
├── Views/                    # صفحات الموقع العام
└── wwwroot/
    ├── lib/                  # مكتبات الواجهة (مُستضافة محليًا)
    ├── css/ js/               # ستايلات وسكريبتات المشروع
    └── images/                # الصور المرفوعة + الصور الافتراضية
```
