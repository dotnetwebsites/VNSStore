using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Interfaces;
using VNSStoreMgmt.Models;

namespace VNSStoreMgmt.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<ApplicationUser> userManager,
                    RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserIdentityResult> CreateInitialUser()
        {
            UserIdentityResult res = new UserIdentityResult();

            var user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@admin.com",
                EmailConfirmed = true,
                IsActive = true,
                Gender = "male"
            };

            var u = _userManager.Users.Any(p => p.UserName == "admin");

            if (u == true)
            {
                res.Message = "Admin user already exists.";
                return res;
            }

            var result = await _userManager.CreateAsync(user, "India@123");
            if (result.Succeeded)
            {
                await AddRoleToSpecificUser(user, "superadmin");
                await AddRoleToSpecificUser(user, "admin");

                var cUser = await _userManager.FindByNameAsync(user.UserName);

                res.Message = "User has been created.";
                res.CreatedUser = cUser;

                return res;
            }

            if (!result.Succeeded)
                res.Message = "Error 1000 : Something went wrong, please contact administrator.";

            return res;
        }

        public async Task AddRoleToSpecificUser(ApplicationUser user, string RoleName)
        {
            IdentityRole identityRole = new IdentityRole
            {
                Name = RoleName
            };

            if (!(await _roleManager.RoleExistsAsync(RoleName)))
            {
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                        await _userManager.AddToRoleAsync(user, RoleName);
                }
            }
            else
            {
                if (!(await _userManager.IsInRoleAsync(user, RoleName)))
                    await _userManager.AddToRoleAsync(user, RoleName);
            }

        }
    }
}
