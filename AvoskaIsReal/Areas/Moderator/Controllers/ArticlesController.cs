using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Moderator.Controllers
{
    public class ArticlesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
