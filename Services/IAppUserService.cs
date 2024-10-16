using Microsoft.AspNetCore.Identity;
using VaxTrack_v1.Models;

namespace VaxTrack_v1.Services
{
    public interface IAppUserService
    {
        public Task<IdentityResult> RegisterUser(UserDetailsModel userDetails);
    }

    public class AppUserService:IAppUserService
    {
        // variable
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // constructor
        public AppUserService(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // method
        public async Task<IdentityResult> RegisterUser(UserDetailsModel userDetails)
        {
            var user = new AppUserModel
            {
                UserName = userDetails.Username,
            };

            var result = await _userManager.CreateAsync(user, userDetails.Password);

            if (result.Succeeded)
            {
                // Check if the role exists, if not, create it
                if (!await _roleManager.RoleExistsAsync("user"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("user"));
                }

                // Add the user to the "user" role
                await _userManager.AddToRoleAsync(user, "user");
            }

            return result;
        }
    }
}