using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VaxTrack_v1.Models;

namespace VaxTrack_v1.Services
{
    // interface: app user service | to serve as service and allowed as injectable
    public interface IAppUserService
    {
        public Task<IdentityResult> RegisterUser(UserDetailsModel userDetails);
    }

    // class: app user service | implementing service methods and handeling utility methods
    public class AppUserService:IAppUserService
    {
        // variable: user manager | for authentication
        private readonly UserManager<AppUserModel> _userManager;

        // variable: role manager | for authentication
        private readonly RoleManager<IdentityRole> _roleManager;

        // contructor: app user service | to initialize account service class variables
        public AppUserService(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /*
        *   service method: RegisterUser()
        *   purpose: to register user with username and password in asp-net-user table
        *   parameter: user details model as object
        *   return: return identity result on successful registration
        */
        public async Task<IdentityResult> RegisterUser(UserDetailsModel userDetails)
        {
            // mapping username fom user details model to app user model
            var user = new AppUserModel
            {
                UserName = userDetails.Username,
            };

            // saving app user model in asp-net-users table
            var result = await _userManager.CreateAsync(user, userDetails.Password);

            // if saving successful
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