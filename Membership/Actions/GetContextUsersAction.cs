using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Membership.MembershipServices;
using Ninject;

namespace Membership.Actions
{
    public class GetContextUsersAction : SABFramework.PreDefinedModules.MembershipModule.Actions.GetUsersAction
    {
        [Inject]
        public IUsersAPI UsersAPI { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            if (SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserIsAdministrator)
            {
                return await base.GetHandler(controller);
            }
            else
            {
                if (String.IsNullOrEmpty(UserName))
                {
                    return Json(UsersAPI.GetUsers());
                }
                else
                {
                    return Json(UsersAPI.GetUser(UserName));
                }
            }
        }
    }
}
