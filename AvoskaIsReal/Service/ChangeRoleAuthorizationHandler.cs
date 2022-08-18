using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AvoskaIsReal.Service
{
    public class ChangeRoleAuthorizationHandler :
        AuthorizationHandler<ChangeRoleAuthorizationRequirement, User>
    {
        private UserManager<User> _userManager;
        private AppUserRoleManager _appRoleManager;
        public ChangeRoleAuthorizationHandler(UserManager<User> userManager,
            AppUserRoleManager appRoleManager)
        {
            _userManager = userManager;
            _appRoleManager = appRoleManager;
        }

        //    Владелец может менять любые роли для любых пользователей,
        //    админ - роли до владельца любым пользователям до владельца,
        //    модератор не может изменять роли.
        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            ChangeRoleAuthorizationRequirement requirement, User resource)
        {
            // Если устанавливается такая же роль, которая и была -
            // авторизация проходит
            string? usersRole = await _appRoleManager.GetRoleAsync(resource);
            if (usersRole is not null && usersRole.Equals(requirement.Role,
                StringComparison.OrdinalIgnoreCase))
            {
                context.Succeed(requirement);
                return;
            }

            User currentUser = await _userManager.GetUserAsync(context.User);

            // Владелец может изменять роли любых пользователей
            if (await _userManager.IsInRoleAsync(currentUser, AppUserRoleManager.OWNER))
            {
                context.Succeed(requirement);
                return;
            }

            // Текущий пользователь - админ (не владелец)
            bool isCurrentUserAdmin = await _userManager.IsInRoleAsync(currentUser,
                AppUserRoleManager.ADMIN);

            // Пользователь - владелец
            bool isUserOwner = await _userManager.IsInRoleAsync(resource,
                AppUserRoleManager.OWNER);

            // Устанавливаемая роль - владелец
            bool isNewRoleOwner = AppUserRoleManager.OWNER.Equals(requirement.Role,
                StringComparison.OrdinalIgnoreCase);

            // Админ может изменять роли модераторам и админам,
            // не может назначать владельцев
            if (isCurrentUserAdmin && !isUserOwner && !isNewRoleOwner)
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
