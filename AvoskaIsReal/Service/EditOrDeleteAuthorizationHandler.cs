using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AvoskaIsReal.Service
{
    public class EditOrDeleteAuthorizationHandler :
        AuthorizationHandler<EditOrDeleteRequirement, User>
    {
        private UserManager<User> _userManager;

        public EditOrDeleteAuthorizationHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Логика частично похожа на логику авторизации смены роли
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            EditOrDeleteRequirement requirement, User user)
        {
            User currentUser = await _userManager.GetUserAsync(context.User);
            // Если пользователь изменяет свой аккаунт, или пользователь - владелец
            if (currentUser.Id == user.Id ||
                await _userManager.IsInRoleAsync(currentUser, AppUserRoleManager.OWNER))
            {
                context.Succeed(requirement);
                return;
            }

            // Текущий пользователь - админ, но не владелец
            bool isCurrentUserAdmin = await _userManager.IsInRoleAsync(currentUser,
                AppUserRoleManager.ADMIN);
            bool isUserOwner = await _userManager.IsInRoleAsync(user,
                AppUserRoleManager.OWNER);

            // Админ может изменять модераторов и админов
            if (isCurrentUserAdmin && !isUserOwner)
            {
                context.Succeed(requirement);
                return;
            }
            else
            {
                context.Fail();
                return;
            }
        }
    }
}
