using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Interfaces;
using VNSStoreMgmt.Models;

namespace VNSStoreMgmt.Controllers.APIControllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserService userService,
            UserManager<ApplicationUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [Route("api/user/createadminuser")]
        public async Task<UserIdentityResult> Index()
        {
            var user = _userManager.Users.FirstOrDefault();
            await _userManager.DeleteAsync(user);

            var result = await _userService.CreateInitialUser();
            return result;
        }
    }
}
