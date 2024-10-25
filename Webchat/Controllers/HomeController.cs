using Microsoft.AspNetCore.Mvc;

namespace Webchat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
