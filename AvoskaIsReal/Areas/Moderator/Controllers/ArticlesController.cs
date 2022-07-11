using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Moderator.Controllers
{
    [Area("moderator")]
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
