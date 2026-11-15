using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Controllers
{
    // صفحة السلة. محتوى السلة نفسه محفوظ في متصفح الزائر (localStorage) عن طريق wwwroot/js/cart.js،
    // وإتمام الطلب بيتم بإرسال كل المنتجات في رسالة واتساب واحدة — الموقع مش بيخزن طلبات في قاعدة البيانات.
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
