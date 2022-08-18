using Microsoft.AspNetCore.Authorization;

namespace AvoskaIsReal.Service
{
    public class ResourceBasedRoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; set; }
        public ResourceBasedRoleRequirement(string role)
        {
            Role = role;
        }
    }
}
