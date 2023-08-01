using Microsoft.AspNetCore.Mvc;
using KUSYS.Application.WebUI.Authorize;

namespace KUSYS.Application.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
