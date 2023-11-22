using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.MembershipServices
{
    public interface IContentPrivilegesAPI: IDisposable
    {
        List<ContentPrivilege> GetPrivileges();
        ContentPrivilege GetPrivilege(int contentId);
        int CreatePrivilege(ContentPrivilege p);
        int UpdatePrivilege(ContentPrivilege p);
        int DeletePrivilege(int contentId);
        int AddPrivilegeToRole(int contentId, string RoleName);
        int RemovePrivilegeFromRole(int contentId, string RoleName);

    }
}
