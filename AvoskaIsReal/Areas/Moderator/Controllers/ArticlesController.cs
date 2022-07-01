using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Moderator.Controllers
{
    public class ArticlesController : Controller
    {
        public IActionResult List()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
