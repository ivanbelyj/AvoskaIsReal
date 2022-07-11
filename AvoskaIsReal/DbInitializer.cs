using Microsoft.AspNetCore.Identity;
using AvoskaIsReal.Domain;
namespace AvoskaIsReal
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            // Add roles
            await HasRole(roleManager, "moderator");
            await HasRole(roleManager, "admin");
            await HasRole(roleManager, "owner");

            // Create owner's account
            string ownerName = configuration.GetSection("Project")["OwnerUserName"];
            string ownerPassword =
                configuration.GetSection("Project")["OwnerInitialPassword"];
            User owner = new User()
            {
                UserName = ownerName
            };
            IdentityResult createRes =
                await userManager.CreateAsync(owner, ownerPassword);
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(owner, "owner");
            }
            else
            {
                // Todo: log error
            }
        }

        private static async Task HasRole(RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                IdentityResult createRes =
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                if (createRes.Errors.Count() != 0)
                {
                    // Todo: log error
                }
            }
        }
    }
}
