using AvoskaIsReal.Areas.Admin.Models;
using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvoskaIsReal.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password == model.ConfirmPassword)
                {
                    User user = new User() { Email = model.Email, UserName = model.Login };
                    IdentityResult createRes = await _userManager
                        .CreateAsync(user, model.Password);
                    if (createRes.Succeeded)
                    {
                        return RedirectToAction("Index");
                    } else
                    {
                        foreach (IdentityError error in createRes.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                } else
                {
                    ModelState.AddModelError("", "Пароли не совпадают.");
                }
            }
            return View(model);
        }
    }
}
