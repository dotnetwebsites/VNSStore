using System.Threading.Tasks;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Models;

namespace VNSStoreMgmt.Interfaces
{
    public interface IUserService
    {
        Task<UserIdentityResult> CreateInitialUser();

        Task AddRoleToSpecificUser(ApplicationUser user, string RoleName);
    }
}
