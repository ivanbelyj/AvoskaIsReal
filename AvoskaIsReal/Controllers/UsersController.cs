using Microsoft.AspNetCore.Mvc;
using AvoskaIsReal.Models;
using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;

namespace AvoskaIsReal.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        private IWebHostEnvironment _webHostEnvironment;
        public UsersController(UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _webHostEnvironment = environment;
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
            }
            else
            {
                user = await _userManager.FindByIdAsync(id);
            }
            if (user != null)
            {
                // Todo: фото профиля по умолчанию
                AccountViewModel model = new AccountViewModel()
                {
                    Login = user.UserName,
                    About = user.About,
                    Career = user.Career,
                    Contacts = user.Contacts,
                    AvatarUrl = user.AvatarUrl
                };
                return View(model);
            }
            return Unauthorized();
        }

        

        public async Task<IActionResult> Edit(string? userId = null)
        {
            User user;
            // Если пользователь не задан, значит, редактируется текущий
            if (userId is null)
                user = await _userManager.GetUserAsync(User);
            else
                user = await _userManager.FindByIdAsync(userId);

            if (user is not null)
            {
                // В форме устанавливается все, кроме пароля. Пароль изменяется отдельно
                // Todo: возможность вернуть аватар по умолчанию
                var model = new EditUserViewModel()
                {
                    Id = user.Id,
                    About = user.About,
                    AvatarUrl = user.AvatarUrl,
                    Career = user.Career,
                    Contacts = user.Contacts,
                    Email = user.Email,
                    Login = user.UserName,
                };
                return View(model);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, IFormFile avatarFile,
            string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                    return NotFound();

                user.About = model.About;
                user.Career = model.Career;
                user.Contacts = model.Contacts;
                user.Email = model.Email;
                user.AvatarUrl = avatarFile.FileName;

                IdentityResult res = await _userManager.UpdateAsync(user);
                if (res.Succeeded)
                {
                    // Если пользователь успешно обновлен, нужно сохранить аватар
                    if (avatarFile is not null)
                    {
                        string path = Path.Combine(_webHostEnvironment.WebRootPath, "images",
                            avatarFile.FileName);

                        // Todo: Обработать ситуацию с одинаковыми именами в папке
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            avatarFile.CopyTo(stream);
                        }
                    }

                    if (returnUrl is not null)
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", new { id = user.Id });
                }
                else
                {
                    foreach (var error in res.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
