using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
