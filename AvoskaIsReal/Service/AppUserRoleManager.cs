using AvoskaIsReal.Domain;
using Microsoft.AspNetCore.Identity;

namespace AvoskaIsReal.Service
{
    public class AppUserRoleManager
    {
        public const string MODERATOR = "moderator";
        public const string ADMIN = "admin";
        public const string OWNER = "owner";

        private UserManager<User> _userManager;
        public AppUserRoleManager(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        async public Task<string?> GetRoleAsync(User user)
        {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            return GetUpperRole(userRoles);
        }

        // Получает наивысшую роль из имеющихся
        private string? GetUpperRole(IList<string> roles)
        {
            bool isModerator = false;
            bool isAdmin = false;

            foreach (string role in roles)
            {
                if (role.Equals(MODERATOR, StringComparison.OrdinalIgnoreCase))
                    isModerator = true;
                else if (role.Equals(ADMIN, StringComparison.OrdinalIgnoreCase))
                    isAdmin = true;
                else if (role.Equals(OWNER, StringComparison.OrdinalIgnoreCase))
                    return OWNER;
            }

            if (isModerator && !isAdmin)
                return MODERATOR;
            if (isAdmin)
                return ADMIN;
            return null;
        }

        async public Task<IdentityResult> SetRoleAsync(User user, string role)
        {
            IList<string> oldRoles = await _userManager.GetRolesAsync(user);
            IdentityResult removeFromRolesRes =
                await _userManager.RemoveFromRolesAsync(user, oldRoles);
            if (!removeFromRolesRes.Succeeded)
                throw new ApplicationException("Не удалось исключить пользователя из "
                    + "предыдущих ролей перед установкой новых.");

            List<string> roles = new List<string>();

            if (EqualRoles(role, MODERATOR))
                roles.Add(MODERATOR);
            if (EqualRoles(role, ADMIN))
            {
                roles.Add(MODERATOR);
                roles.Add(ADMIN);
            }
            if (EqualRoles(role, OWNER))
            {
                roles.Add(MODERATOR);
                roles.Add(ADMIN);
                roles.Add(OWNER);
            }

            return await _userManager.AddToRolesAsync(user, roles);
        }

        private bool EqualRoles(string role1, string role2) => role1.Equals(role2,
            StringComparison.OrdinalIgnoreCase);
    }
}
