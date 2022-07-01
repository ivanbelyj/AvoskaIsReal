using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class AccountController : Controller
    {
        // Профиль пользователя
        public IActionResult Index(string id)
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult LogIn()
        {
            return View();
        }
    }
}
