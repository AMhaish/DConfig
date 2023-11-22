using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public interface IPrivilegeAPI
    {
        List<Privilege> GetPrivileges();
        Privilege GetPrivilege(int id);
        bool CreatePrivilege(Privilege p);
        bool UpdatePrivilege(Privilege p);
        bool DeletePrivilege(int id);
        bool AddPrivilegeToRole(int PrivilegeId, string RoleName);
        bool RemovePrivilegeFromRole(int PrivilegeId, string RoleName);

    }
}
