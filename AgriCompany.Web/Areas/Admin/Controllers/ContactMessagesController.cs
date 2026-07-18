using AgriCompany.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    public class ContactMessagesController : AdminBaseController
    {
        private readonly IContactMessageService _contactMessageService;

        public ContactMessagesController(IContactMessageService contactMessageService)
        {
            _contactMessageService = contactMessageService;
        }

        public async Task<IActionResult> Index(bool? unreadOnly, int page = 1)
        {
            var paged = await _contactMessageService.GetPagedAsync(page, unreadOnly);
            ViewBag.UnreadOnly = unreadOnly;
            return View(paged);
        }

        public async Task<IActionResult> Details(int id)
        {
            var message = await _contactMessageService.GetByIdAsync(id, markAsRead: true);
            if (message == null) return NotFound();
            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _contactMessageService.DeleteAsync(id);
            TempData["Success"] = "تم حذف الرسالة بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
