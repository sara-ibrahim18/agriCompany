using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriCompany.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Base controller for the entire Admin area. Every controller under Areas/Admin inherits from
    /// this and therefore requires authentication (Admin role) EXCEPT AccountController, which
    /// overrides this via [AllowAnonymous] on its login action.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminBaseController : Controller
    {
    }
}
