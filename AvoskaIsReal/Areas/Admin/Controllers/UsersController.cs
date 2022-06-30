using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
