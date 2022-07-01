using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class ArticlesController : Controller
    {

        public IActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View();
            }
            return id == "id" ? View("Show") : Content("");
        }

        public IActionResult AllAboutAvoska()
        {
            return View();
        }


    }
}
