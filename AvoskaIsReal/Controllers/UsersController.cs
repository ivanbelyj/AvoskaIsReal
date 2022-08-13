using Microsoft.AspNetCore.Mvc;
using AvoskaIsReal.Models;
using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;

namespace AvoskaIsReal.Controllers
{
    /*
    Todo: Возможность изменять роли пользователя.
    Владелец может менять любые роли для любых пользователей,
    админ - роли до владельца любым пользователям до владельца,
    модератор не может изменять роли.
    */
    [Authorize]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        private IWebHostEnvironment _webHostEnvironment;

        // Требуется в случае удаления пользователя (перед удалением нужно выйти)
        private SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager,
            IWebHostEnvironment environment, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _webHostEnvironment = environment;
            _signInManager = signInManager;
        }

        // Если пользователь изменяет свой аккаунт,
        // либо он авторизован на редактирование чужого
        // (админ, либо владелец, если требуется редактировать админа)
        public async Task<bool> IsAllowedToEditOrDelete(User editor, User editing)
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
        // returnUrl - адрес возврата, если пользователь будет удален
        [AllowAnonymous]
        public async Task<IActionResult> Index(string id, string returnUrl)
        {
            // id всегда должен передаваться
            // todo: убрать
            if (id == null)
                throw new Exception("id == null !");

            // Отображаемый пользователь
            User? user = null;

            // Если не дано id, но пользователь авторизован
            //if (id is null && User.Identity is not null && User.Identity.IsAuthenticated)
            //{
            //    user = await _userManager.GetUserAsync(User);
            //}
            // Иначе пользователь дан по id
            /*else if (id is not null)*/
            //{
                user = await _userManager.FindByIdAsync(id);
            //}

            // Если пользователь для отображения так и не нашелся, ошибка
            if (user == null)
                return NotFound();

            // Иначе пользователь определен

            // Определить, нужно ли отображать ссылку на редактирование пользователя.
            bool showEditLink = false;

            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is not null)
            {
                showEditLink = await IsAllowedToEditOrDelete(currentUser, user);
            }

            ViewBag.showEditLink = showEditLink;

            // Если пользователь просматривает свой профиль, отобразить кнопку
            // для выхода из аккаунта
            if (currentUser is not null && currentUser.Id == user.Id)
            {
                ViewBag.showLogOutButton = true;
            }
            ViewBag.deleteReturnUrl = returnUrl;

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

        public async Task<IActionResult> Edit(string id, string? deleteReturnUrl = null,
            string? changePasswordReturnUrl = null)
        {
            // id всегда должен передаваться
            // todo: убрать
            if (id == null)
                throw new Exception("id == null !");

            User user = await _userManager.FindByIdAsync(id);

            if (user is not null)
            {
                // Извне контроллера определяется маршрут возврата после
                // удаления пользователя
                ViewBag.deleteReturnUrl = deleteReturnUrl;

                // Если пользователь может изменить пароль, отобразить ссылку
                User currentUser = await _userManager.GetUserAsync(User);
                ViewBag.showEditPassword = await IsAllowedToChangePassword(currentUser, user);

                ViewBag.changePasswordReturnUrl = changePasswordReturnUrl;
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

        /// <summary>
        /// Производится проверка, имеет ли право текущий пользователь
        /// совершать операции над аккаунтом другого пользователя.
        /// В случае успешной проверки выполняется действие.
        /// Оба пользователя должны быть определены, иначе возвращаются
        /// http коды ошибок.
        /// </summary>
        private async Task<IActionResult> AllowedAction(string otherUserId,
            Func<User, User, Task<bool>> checkIsAllowed,
            Func<User, Task<IActionResult>> onSuccess)
        {
            User otherUser = await _userManager.FindByIdAsync(otherUserId);

            // Другой пользователь не найден
            if (otherUser is null)
                return NotFound();

            User currentUser = await _userManager.GetUserAsync(User);
            // Для неаутентифицированных пользователей
            if (currentUser is null)
                return Unauthorized();

            if (!await checkIsAllowed(currentUser, otherUser))
                return Unauthorized();

            return await onSuccess(otherUser);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, IFormFile avatarFile,
            string? deleteReturnUrl = null)
        {
            return await AllowedAction(model.Id, IsAllowedToEditOrDelete,
                async (user) =>
            {
                if (ModelState.IsValid)
                {
                    // user.Id = model.Id;
                    user.About = model.About;
                    user.Career = model.Career;
                    user.Contacts = model.Contacts;
                    user.Email = model.Email;
                    user.UserName = model.Login;
                    user.AvatarUrl = avatarFile.FileName;
                    

                    IdentityResult res = await _userManager.UpdateAsync(user);
                    if (res.Succeeded)
                    {
                        // Если пользователь успешно обновлен и у него был
                        // установлен новый аватар,
                        // нужно сохранить аватар
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

                        if (deleteReturnUrl is not null)
                            return Redirect(deleteReturnUrl);
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
            });
        }

        public async Task<IActionResult> Delete(string id, string returnUrl)
        {
            return await AllowedAction(id, IsAllowedToEditOrDelete,
                async (user) =>
            {
                User currentUser = await _userManager.GetUserAsync(User);
                // Если пользователь удаляет сам себя, то нужно выйти из аккаунта
                if (currentUser.Id == id)
                    await _signInManager.SignOutAsync();

                IdentityResult deleteRes = await _userManager.DeleteAsync(user);
                if (returnUrl is not null)
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index");
            });
        }

        public async Task<bool> IsAllowedToChangePassword(User changer,
            User otherUser)
        {
            // Можно разрешать изменять чужой пароль некоторым пользователям,
            // но в данном случае его может изменять тот, кто может изменять аккаунт,
            // при условии подтверждения старого пароля
            return await IsAllowedToEditOrDelete(changer, otherUser);

            //// Пользователь может менять свой пароль
            //if (changer.Id == otherUser.Id)
            //    return true;

            //// Админ может изменять пароль не админу
            //bool isChangerAdmin = await _userManager.IsInRoleAsync(changer, "admin");
            //bool isOtherUserAdmin = await _userManager.IsInRoleAsync(otherUser, "admin");
            //if (isChangerAdmin && !isOtherUserAdmin)
            //    return true;

            //// Владелец может менять пароль любому
            //bool isChangerOwner = await _userManager.IsInRoleAsync(changer, "owner");
            //if (isChangerOwner)
            //    return true;

            //return false;
        }

        // Изменение пароля пользователя
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId,
            string? returnUrl = null)
        {
            User currentUser = await _userManager.GetUserAsync(User);
            User otherUser = await _userManager.FindByIdAsync(userId);
            if (currentUser is null)
                return Unauthorized();
            if (otherUser is null)
                return NotFound();

            return View(new ChangePasswordViewModel() { UserId = userId,
                ReturnUrl = returnUrl });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            return await AllowedAction(model.UserId, IsAllowedToChangePassword,
                async (User user) =>
                {
                    if (ModelState.IsValid)
                    {
                        if (model.NewPassword == model.ConfirmNewPassword)
                        {
                            IdentityResult res = await _userManager
                                .ChangePasswordAsync(user, model.OldPassword,
                                    model.NewPassword);
                            if (res.Succeeded)
                            {
                                // return RedirectToAction("Edit", "Users", new { user.Id });
                                if (model.ReturnUrl is not null)
                                    return Redirect(model.ReturnUrl);
                                else
                                    return RedirectToAction("Index", new { id = model.UserId });
                            }
                            else
                            {
                                foreach (var error in res.Errors)
                                {
                                    ModelState.AddModelError("", error.Description);
                                }
                            }
                        } else
                        {
                            ModelState.AddModelError("", "Пароли не совпадают");
                        }
                    }
                    return View(model);
                });
        }
    }
}
