using DConfigOS_Core.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.MembershipServices
{
    public interface IUsersAPI : IDisposable
    {
        List<ApplicationUser> GetUsers();
        ApplicationUser GetUser(string userName);
        ApplicationUser GetUserById(string userId);
        List<IdentityUserRole> GetUserRoles(string userId);
        List<ApplicationUser> GetUsersByName(string userName);
        int CreateExUser(ExApplicationUser user);
        int UpdateExUser(ExApplicationUser user);
    }
}
