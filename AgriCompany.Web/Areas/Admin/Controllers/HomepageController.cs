//using AgriCompany.Web.Models;
//using AgriCompany.Web.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace AgriCompany.Web.Areas.Admin.Controllers
//{
//    public class HomepageController : AdminBaseController
//    {
//        private readonly IHomepageService _homepageService;

//        public HomepageController(IHomepageService homepageService)
//        {
//            _homepageService = homepageService;
//        }

//        public async Task<IActionResult> Index()
//        {
//            var slides = await _homepageService.GetAllSlidesAsync();
//            return View(slides);
//        }

//        public IActionResult Create()
//        {
//            return View(new HomepageSlide());
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
//        {
//            ModelState.Remove(nameof(slide.ImagePath));   // <-- السطر الجديد

//            if (image == null || image.Length == 0)
//            {
//                ModelState.AddModelError("", "من فضلك اختر صورة للسلايد");
//            }

//            if (!ModelState.IsValid)
//            {
//                return View(slide);
//            }

//            await _homepageService.CreateAsync(slide, image);
//            TempData["Success"] = "تم إضافة السلايد بنجاح";
//            return RedirectToAction(nameof(Index));
//        }
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
//        //{
//        //    // Validation بسيطة
//        //    if (image == null || image.Length == 0)
//        //    {
//        //        ModelState.AddModelError("", "من فضلك اختر صورة للسلايد");
//        //    }

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return View(slide);
//        //    }

//        //    await _homepageService.CreateAsync(slide, image);
//        //    TempData["Success"] = "تم إضافة السلايد بنجاح";
//        //    return RedirectToAction(nameof(Index));
//        //}

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
//        //{
//        //    if (image == null || image.Length == 0)
//        //        ModelState.AddModelError(nameof(image), "من فضلك اختر صورة للسلايد");

//        //    if (!ModelState.IsValid)
//        //        return View(slide);

//        //    await _homepageService.CreateAsync(slide, image);
//        //    TempData["Success"] = "تم إضافة السلايد بنجاح";
//        //    return RedirectToAction(nameof(Index));
//        //}
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
//        //{
//        //    if (image == null || image.Length == 0)
//        //    {
//        //        ModelState.AddModelError("image", "من فضلك اختر صورة للسلايد");   // غير nameof(image) إلى "image"
//        //    }

//        //    if (!ModelState.IsValid)
//        //        return View(slide);

//        //    await _homepageService.CreateAsync(slide, image);
//        //    TempData["Success"] = "تم إضافة السلايد بنجاح";
//        //    return RedirectToAction(nameof(Index));
//        //}
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
//        //{
//        //    if (image == null || image.Length == 0)
//        //    {
//        //        ModelState.AddModelError("image", "من فضلك اختر صورة للسلايد");
//        //    }

//        //    if (!ModelState.IsValid)
//        //    {
//        //        return View(slide);
//        //    }

//        //    await _homepageService.CreateAsync(slide, image);
//        //    TempData["Success"] = "تم إضافة السلايد بنجاح";
//        //    return RedirectToAction(nameof(Index));
//        //}
//        public async Task<IActionResult> Edit(int id)
//        {
//            var slide = await _homepageService.GetByIdAsync(id);
//            if (slide == null) return NotFound();
//            return View(slide);
//        }

//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public async Task<IActionResult> Edit(int id, HomepageSlide slide, IFormFile? image)
//        //{
//        //    if (id != slide.Id) return NotFound();
//        //    if (!ModelState.IsValid) return View(slide);

//        //    await _homepageService.UpdateAsync(slide, image);
//        //    TempData["Success"] = "تم تحديث السلايد بنجاح";
//        //    return RedirectToAction(nameof(Index));
//        //}
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, HomepageSlide slide, IFormFile? image)
//        {
//            if (id != slide.Id) return NotFound();

//            // إزالة أي validation errors مخفية
//            ModelState.Remove("TitleEn");
//            ModelState.Remove("SubtitleEn");
//            ModelState.Remove("ImagePath");   // لأن الصورة optional

//            if (!ModelState.IsValid)
//            {
//                return View(slide);
//            }

//            var result = await _homepageService.UpdateAsync(slide, image);
//            if (result)
//            {
//                TempData["Success"] = "تم تحديث السلايد بنجاح";
//            }
//            else
//            {
//                TempData["Error"] = "فشل تحديث السلايد";
//            }

//            return RedirectToAction(nameof(Index));
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _homepageService.DeleteAsync(id);
//            TempData["Success"] = "تم حذف السلايد بنجاح";
//            return RedirectToAction(nameof(Index));
//        }
//    }
//}
using AgriCompany.Web.Models;
using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class HomepageController : AdminBaseController
    {
        private readonly IHomepageService _homepageService;

        public HomepageController(IHomepageService homepageService)
        {
            _homepageService = homepageService;
        }

        public async Task<IActionResult> Index()
        {
            var slides = await _homepageService.GetAllSlidesAsync();
            return View(slides);
        }

        public IActionResult Create()
        {
            return View(new HomepageSlide());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HomepageSlide slide, IFormFile? image)
        {
            ModelState.Remove(nameof(slide.ImagePath));
            if (image == null || image.Length == 0)
                ModelState.AddModelError(nameof(image), "من فضلك اختر صورة للسلايد");

            if (!ModelState.IsValid)
                return View(slide);

            try
            {
                await _homepageService.CreateAsync(slide, image);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(slide);
            }

            TempData["Success"] = "تم إضافة السلايد بنجاح";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var slide = await _homepageService.GetByIdAsync(id);
            if (slide == null) return NotFound();
            return View(slide);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, HomepageSlide slide, IFormFile? image)
        //{
        //    if (id != slide.Id) return NotFound();
        //    if (!ModelState.IsValid) return View(slide);

        //    try
        //    {
        //        await _homepageService.UpdateAsync(slide, image);
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //        return View(slide);
        //    }

        //    TempData["Success"] = "تم تحديث السلايد بنجاح";
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HomepageSlide slide, IFormFile? image)
        {
            if (id != slide.Id) return NotFound();

            // إزالة أي validation errors مخفية
            ModelState.Remove("TitleEn");
            ModelState.Remove("SubtitleEn");
            ModelState.Remove("ImagePath");   // لأن الصورة optional

            if (!ModelState.IsValid)
            {
                return View(slide);
            }

            var result = await _homepageService.UpdateAsync(slide, image);
            if (result)
            {
                TempData["Success"] = "تم تحديث السلايد بنجاح";
            }
            else
            {
                TempData["Error"] = "فشل تحديث السلايد";
            }

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _homepageService.DeleteAsync(id);
            TempData["Success"] = "تم حذف السلايد بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}