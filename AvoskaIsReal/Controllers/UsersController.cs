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
        public UsersController(UserManager<User> userManager,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _webHostEnvironment = environment;
        }

        // Если пользователь изменяет свой аккаунт,
        // либо он авторизован на редактирование чужого
        // (админ, либо владелец, если требуется редактировать админа)
        public async Task<bool> IsAllowedToEdit(User editor, User editing)
        {
            // Если пользователь изменяет свой аккаунт
            if (editor.Id == editing.Id)
                return true;

            // Если админ хочет изменить обычного пользователя
            bool isEditingAdmin = await _userManager.IsInRoleAsync(editing, "admin");
            bool isEditorUserAdmin = await _userManager.IsInRoleAsync(editor,
                "admin");

            // Владельцу можно все
            bool isCurrentUserOwner = await _userManager.IsInRoleAsync(editor,
                "owner");
            return isEditorUserAdmin && !isEditingAdmin || isCurrentUserOwner;
        }

        // Профиль пользователя
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? id = null)
        {
            // Отображаемый пользователь
            User? user = null;

            // Если не дано id, но пользователь авторизован
            if (id is null && User.Identity is not null && User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            // Иначе пользователь дан по id
            else if (id is not null)
            {
                user = await _userManager.FindByIdAsync(id);
            }
            // Если пользователь для отображения так и не нашелся, ошибка
            if (user == null)
                return NotFound();
            // Иначе пользователь определен

            // Определить, нужно ли отображать ссылку на редактирование пользователя.
            bool showEditLink = false;

            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is not null)
            {
                showEditLink = await IsAllowedToEdit(currentUser, user);
            }

            ViewBag.showEditLink = showEditLink;

            // Todo: фото профиля по умолчанию
            ShowAccountViewModel model = new ShowAccountViewModel()
            {
                Id = user.Id,
                Login = user.UserName,
                About = user.About,
                Career = user.Career,
                Contacts = user.Contacts,
                AvatarUrl = user.AvatarUrl
            };
            return View(model);
        }

        public async Task<IActionResult> Edit(string? id = null)
        {
            User user;
            // Если пользователь не задан, значит, редактируется текущий
            if (id is null)
                user = await _userManager.GetUserAsync(User);
            else
                user = await _userManager.FindByIdAsync(id);

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
            User user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is null)
                return Unauthorized();

            if (!await IsAllowedToEdit(currentUser, user))
                return Unauthorized();

            if (ModelState.IsValid)
            {
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

        // Изменение пароля текущего пользователя
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel() { });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.GetUserAsync(User);

                IdentityResult res = await _userManager.ChangePasswordAsync(user, model.OldPassword,
                    model.NewPassword);
                if (res.Succeeded)
                {
                    return RedirectToAction("Edit", "Users");
                } else
                {
                    foreach (var error in res.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}
