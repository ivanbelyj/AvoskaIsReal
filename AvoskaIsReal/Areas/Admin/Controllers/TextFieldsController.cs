using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    [Area("admin")]
    public class TextFieldsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
