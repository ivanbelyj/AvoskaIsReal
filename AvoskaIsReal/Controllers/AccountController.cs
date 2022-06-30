using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult LogIn()
        {
            return View();
        }
    }
}
