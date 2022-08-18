using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace AvoskaIsReal.Service
{
    /// <summary>
    /// Проверка роли пользователя, основываясь на БД, а не claims
    /// </summary>
    public class ResourceBasedRoleHandler :
        AuthorizationHandler<ResourceBasedRoleRequirement, User>
    {
        private UserManager<User> _userManager;
        public ResourceBasedRoleHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceBasedRoleRequirement requirement, User resource)
        {
            if (await _userManager.IsInRoleAsync(resource, requirement.Role))
            {
                context.Succeed(requirement);
            } else
            {
                context.Fail();
            }
        }
    }
}
