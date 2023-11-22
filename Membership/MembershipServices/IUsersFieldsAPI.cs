using DConfigOS_Core.Models;
using Membership.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Membership.MembershipServices
{
    public interface IUsersFieldsAPI
    {
        List<UserField> GetUsersFields();
        List<FieldsType> GetUserFieldTypes();
        int AddUserField(UserField field);
        int UpdateUserField(UserField field);
        int DeleteUserField(int fieldId);
        int UpdateUserFieldValue(string userId, int fieldId, string value);
        List<UserField> GetUserFieldsValues(string userId);

    }
}
