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
        public async Task<IActionResult> Index(string? id = null)
        {
            User user;
            if (id is null)
            {
                // По умолчанию - профиль текущего аутентифицированного пользователя
                user = await _userManager.GetUserAsync(User);
            } else
            {
                user = await _userManager.FindByIdAsync(id);
            }
            if (user != null)
            {
                AccountViewModel model = new AccountViewModel() {
                    Login = user.UserName,
                    About = user.About,
                    Career = user.Career,
                    Contacts = user.Contacts,
                    AvatarUrl = user.AvatarUrl
                };
                return View("Show", model);
            }
            return Unauthorized();
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
                    // Todo: remember me не работает
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

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
