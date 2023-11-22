using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.MembershipServices
{
    public interface IUserFieldsEnumsAPI : IDisposable
    {
        List<UserFieldEnum> GetUserFieldEnums();
        UserFieldEnum GetUserFieldEnum(int id);
        int CreateUserFieldEnum(UserFieldEnum userFieldEnum);
        int UpdateUserFieldEnum(UserFieldEnum userFieldEnum);
        int DeleteUserFieldEnum(int id);
        int UpdateUserFieldEnumValues(int enumId, List<string> values);
        int DeleteUserFieldEnumValue(int id);

    }
}
