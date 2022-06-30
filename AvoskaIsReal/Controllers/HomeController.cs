using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Articles()
        {
            return View();
        }

        public IActionResult AllAboutAvoska()
        {
            return View();
        }

        public IActionResult Article(string id)
        {
            return id == "id"? View() : Content("");
        }
    }
}
