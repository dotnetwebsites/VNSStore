using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VNSStoreMgmt.Areas.Identity.Data;

namespace VNSStoreMgmt.Models
{
    public class UserViewModel
    {

    }

    public class UserIdentityResult : APIResult
    {
        public ApplicationUser CreatedUser { get; set; }
    }
}
