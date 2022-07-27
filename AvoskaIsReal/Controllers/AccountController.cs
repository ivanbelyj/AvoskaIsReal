using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AvoskaIsReal.Models;
using AvoskaIsReal.Domain;

namespace AvoskaIsReal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private SignInManager<User> _signInManager;
        private UserManager<User> _userManager;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Профиль пользователя
        [AllowAnonymous]
        public IActionResult Index(string id)
        {
            return View();
        }

        // returnUrl для случая, если пользователя перенаправили на вход при его попытке
        // неавторизованных действий
        [AllowAnonymous]
        public IActionResult LogIn(string? returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;
            return View(new LogInViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult res = await _signInManager
                        .PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                    if (res.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "Неверный email или пароль");
            }
            return View(model);
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
