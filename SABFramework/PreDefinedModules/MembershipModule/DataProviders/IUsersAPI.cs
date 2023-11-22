using Microsoft.AspNet.Identity.EntityFramework;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public interface IUsersAPI
    {
        List<ApplicationUser> GetUsers();
        List<ApplicationUser> GetCompanyUsers(int companyId);
        ApplicationUser GetUser(string userName);
        ApplicationUser GetUserById(string userId);
        List<IdentityUserRole> GetUserRoles(string userId);
        List<ApplicationUser> GetUsersByName(string userName);

    }
}
