using Microsoft.AspNetCore.Mvc;
using AvoskaIsReal.Models;
using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;
using AvoskaIsReal.Service;

namespace AvoskaIsReal.Controllers
{
    /*
    
    */
    [Authorize]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        private AppUserRoleManager _appRoleManager;
        private IWebHostEnvironment _webHostEnvironment;
        private IAuthorizationService _authorizationService;

        // Требуется в случае удаления пользователя (перед удалением нужно выйти)
        private SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager,
            IWebHostEnvironment environment, SignInManager<User> signInManager,
            AppUserRoleManager appRoleManager, IAuthorizationService authorizationService)
        {
            _userManager = userManager;
            _webHostEnvironment = environment;
            _signInManager = signInManager;
            _appRoleManager = appRoleManager;
            _authorizationService = authorizationService;
        }

        // Профиль пользователя
        // returnUrl - адрес возврата, если пользователь будет удален
        [AllowAnonymous]
        public async Task<IActionResult> Index(string id, string returnUrl)
        {
            if (id == null)
                return BadRequest();

            // Отображаемый пользователь
            User? user = null;

            user = await _userManager.FindByIdAsync(id);

            // Если пользователь для отображения так и не нашелся, ошибка
            if (user == null)
                return NotFound();

            // Определить, нужно ли отображать ссылку на редактирование пользователя.
            bool showEditLink = false;

            User currentUser = await _userManager.GetUserAsync(User);
            if (currentUser is not null)
            {
                // showEditLink = await IsAllowedToEditOrDelete(currentUser, user);
                showEditLink = (await _authorizationService.AuthorizeAsync(User, user,
                    "EditOrDeleteUserPolicy")).Succeeded;
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
            if (id == null)
                return BadRequest();

            User user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            bool isAuthorizedToEdit = (await _authorizationService.AuthorizeAsync(User,
                user, "EditOrDeleteUserPolicy")).Succeeded;
            if (isAuthorizedToEdit)
            {
                // Извне контроллера определяется маршрут возврата после
                // удаления пользователя
                ViewBag.deleteReturnUrl = deleteReturnUrl;

                // Если пользователь может изменить пароль, отобразить ссылку
                // User currentUser = await _userManager.GetUserAsync(User);
                // ViewBag.showEditPassword = (await _authorizationService
                //     .AuthorizeAsync(User, user, "EditOrDeleteUserPolicy")).Succeeded;
                //ViewBag.showDelete = await IsAllowedToEditOrDelete(currentUser, user);

                ViewBag.changePasswordReturnUrl = changePasswordReturnUrl;

                string? role = await _appRoleManager.GetRoleAsync(user);

                // У всех пользователей в приложении должна быть роль
                if (role is null)
                    throw new ApplicationException("Пользователь не имеет роли");

                // Роль на странице редактирования отображается всегда
                // bool showChangeRole = (await _authorizationService.AuthorizeAsync(User, user,
                //     new ChangeRoleAuthorizationRequirement(role))).Succeeded;
                // ViewBag.showChangeRole = showChangeRole;

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
                    Role = role
                };
                return View(model);
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // Todo: Преждевременный отзыв cookies.
        // Сейчас, например, когда пользователь был понижен до модератора,
        // он может получить доступ к списку пользователей и может
        // редактировать контент страниц, т.к. авторизация по ролям привязана к cookies.
        // Редактировать других пользователей он,
        // тем не менее, не может, т.к. эта авторизация - resource-based.
        // Если выйти из аккаунта, а потом снова войти, старые cookies отзываются.
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, IFormFile avatarFile,
            string? deleteReturnUrl = null)
        {
            User user = await _userManager.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();

            // Если пользователь авторизован на изменение данного пользователя
            // и изменение его роли
            bool isAuthorizedToEdit = (await _authorizationService.AuthorizeAsync(User,
                user, "EditOrDeleteUserPolicy")).Succeeded;
            bool isAuthorizedToChangeRole = (await _authorizationService
                .AuthorizeAsync(User, user,
                new ChangeRoleAuthorizationRequirement(model.Role))).Succeeded;
            if (isAuthorizedToEdit && isAuthorizedToChangeRole)
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
                        // Если пользователь успешно обновлен и у него установлен
                        // новый аватар, его нужно сохранить
                        if (avatarFile is not null)
                        {
                            string path = Path.Combine(_webHostEnvironment.WebRootPath,
                                "images", avatarFile.FileName);

                            // Todo: Обработать ситуацию с одинаковыми именами в папке
                            using (FileStream stream = new FileStream(path, FileMode.Create))
                            {
                                await avatarFile.CopyToAsync(stream);
                            }
                        }

                        // Установка роли
                        IdentityResult setRoleRes = await _appRoleManager
                            .SetRoleAsync(user, model.Role);
                        if (setRoleRes.Succeeded)
                        {
                            // Все действия выполнены успешно, перенаправление
                            if (deleteReturnUrl is not null)
                                return Redirect(deleteReturnUrl);
                            else
                                return RedirectToAction("Index", new { id = user.Id });
                        }
                        else
                        {
                            foreach (IdentityError error in setRoleRes.Errors)
                                ModelState.AddModelError("", error.Description);
                        }
                    }
                    else
                    {
                        foreach (var error in res.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                return View(model);
            }
            else
            {
                return new ChallengeResult();
            }
        }

        public async Task<IActionResult> Delete(string id, string returnUrl)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user is null)
                return NotFound();

            if ((await _authorizationService.AuthorizeAsync(User, user,
                "EditOrDeleteUserPolicy")).Succeeded)
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
            }
            else
            {
                return new ChallengeResult();
            }
        }

        // Изменение пароля пользователя
        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId,
            string? returnUrl = null)
        {
            User otherUser = await _userManager.FindByIdAsync(userId);
            if (otherUser is null)
                return NotFound();

            if ((await _authorizationService
                .AuthorizeAsync(User, otherUser, "EditOrDeleteUserPolicy")).Succeeded)
            {
                User currentUser = await _userManager.GetUserAsync(User);

                if (currentUser is null)
                    return Unauthorized();
                if (otherUser is null)
                    return NotFound();

                return View(new ChangePasswordViewModel()
                {
                    UserId = userId,
                    ReturnUrl = returnUrl
                });
            }
            else
            {
                return new ChallengeResult();
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            User user = await _userManager.FindByIdAsync(model.UserId);
            if ((await _authorizationService
                .AuthorizeAsync(User, user, "EditOrDeleteUserPolicy")).Succeeded)
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
                    }
                    else
                    {
                        ModelState.AddModelError("", "Пароли не совпадают");
                    }
                }
                return View(model);
            }
            else
            {
                return new ChallengeResult();
            }
        }
    }
}
