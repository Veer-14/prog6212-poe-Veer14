using Microsoft.AspNetCore.Mvc;

namespace PROG6212_POE.Controllers
{
    public class HomePageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
