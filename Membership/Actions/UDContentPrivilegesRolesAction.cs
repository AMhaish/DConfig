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
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace Membership.Actions
{
    public class UDContentPrivilegesRolesAction : SABFramework.Core.SABAction
    {
        public string RoleName { get; set; }
        public int? ContentId { get; set; }

        [Inject]
        public IContentPrivilegesAPI ContentPrivilegesAPI { get; set; }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (ContentId.HasValue && !String.IsNullOrEmpty(RoleName))
            {
                var result = ContentPrivilegesAPI.AddPrivilegeToRole(ContentId.Value, RoleName);
                if (result==ResultCodes.Succeed)
                {

                    return Json(new { result = "true" });

                }
                else
                {
                    return Json(new { result = "false", message = "Couldn't find either privilege or role" });
                }
            }
            return null;
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (ContentId.HasValue && !String.IsNullOrEmpty(RoleName))
            {
                var result = ContentPrivilegesAPI.RemovePrivilegeFromRole(ContentId.Value, RoleName);
                if (result==ResultCodes.Succeed)
                {

                    return Json(new { result = "true" });

                }
                else
                {
                    return Json(new { result = "false", message = "Couldn't find either privilege or role" });
                }
            }
            return null;
        }
    }
}
