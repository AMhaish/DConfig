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
using Microsoft.Owin.Security;
using System.Security.Claims;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Membership.MembershipServices;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace Membership.Actions
{
    public class CUDContextUsersAction : SABFramework.PreDefinedModules.MembershipModule.Actions.CUDUsersAction
    {
        [Inject]
        public IUsersAPI UsersAPI { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            var actionResult = await base.PostHandler(controller);
            if (actionResult.Model.GetType().GetProperty("result") != null && actionResult.Model.GetType().GetProperty("result").GetValue(actionResult.Model).ToString() == "true")
            {
                var user = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UsersAPI.GetUser(UserName);
                ExApplicationUser exuser = new ExApplicationUser();
                if (SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserIsAdministrator)
                {
                    exuser.ContextCompanyId = null;
                }
                exuser.ContextCompanyId = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId;
                exuser.UserId = user.Id;
                exuser.CreatedDate = DateTime.Now;
                var result = UsersAPI.CreateExUser(exuser);
                if (result == ResultCodes.Succeed)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "Coudln't create the user" });
                }
            }
            return actionResult;
        }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            var user = UsersAPI.GetUser(UserName);
            if (user != null)
            {
                return await base.PutHandler(controller);
            }
            else
            {
                return Json(new { result = "false", message = "This user is not more avialable in this context to be updated." });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var user = UsersAPI.GetUser(UserName);
            if (user != null)
            {
                return await base.DeleteHandler(controller);
            }
            else
            {
                return Json(new { result = "false", message = "This user is not more avialable in this context to be deleted." });
            }
        }
    }
}
