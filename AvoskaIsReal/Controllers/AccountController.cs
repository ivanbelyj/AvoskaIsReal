using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AvoskaIsReal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        // Профиль пользователя
        [AllowAnonymous]
        public IActionResult Index(string id)
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult LogOut()
        {
            return View();
        }
    }
}
