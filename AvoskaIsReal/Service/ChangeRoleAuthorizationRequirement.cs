using Microsoft.AspNetCore.Authorization;
namespace AvoskaIsReal.Service
{
    public class ChangeRoleAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Роль, на установку которой проверяется право пользователя
        /// </summary>
        public string Role { get; set; }

        public ChangeRoleAuthorizationRequirement()
        {

        }
        public ChangeRoleAuthorizationRequirement(string role)
        {
            Role = role;
        }
    }
}