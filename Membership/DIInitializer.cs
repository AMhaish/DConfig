using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Core;
using Membership.MembershipServices;

namespace Membership
{
    public class DIInitializer : NinjectModule
    {
        public override void Load()
        {
            Bind<IContentPrivilegesAPI>().To<ContentPrivilegesAPI>();
            Bind<IUserFieldsEnumsAPI>().To<UserFieldEnumsAPI>();
            Bind<IUsersAPI>().To<UsersAPI>();
            Bind<IUsersFieldsAPI>().To<UsersFieldsAPI>();
        }
    }
}
