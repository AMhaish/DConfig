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
using Membership.Models;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace Membership.Actions
{
    public class CUDContentPrivilegesAction : SABFramework.Core.SABAction
    {
        public int? ContentId { get; set; }
        public bool NeedAuthentication { get; set; }
        public bool NeedAuthorization { get; set; }
        public bool RequireHttps { get; set; }

        [Inject]
        private IContentPrivilegesAPI ContentPrivilegesAPI { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            
            if (ContentId.HasValue)
            {
                var p = new ContentPrivilege() { ContentId = ContentId.Value, NeedAuthentication = NeedAuthentication, NeedAuthorization = NeedAuthorization, RequireHttps = RequireHttps };
                var result = ContentPrivilegesAPI.CreatePrivilege(p);
                var obj = ContentPrivilegesAPI.GetPrivilege(p.ContentId);
                if (result==ResultCodes.Succeed)
                {
                    return Json(new { result = "true", obj=obj });
                }
                else
                {
                    return Json(new { result = "false", message = "Privilege already exist" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Content id is required to create privilege" });
            }
        }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (ContentId.HasValue)
            {
                var p = new ContentPrivilege() { ContentId = ContentId.Value, NeedAuthentication = NeedAuthentication, NeedAuthorization = NeedAuthorization, RequireHttps = RequireHttps };
                var result = ContentPrivilegesAPI.UpdatePrivilege(p);
                var obj = ContentPrivilegesAPI.GetPrivilege(p.ContentId);
                if (result==ResultCodes.Succeed)
                {
                    return Json(new { result = "true", obj=obj });
                }
                else
                {
                    return Json(new { result = "false", message = "Privilege wasn't found" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Content id is required to update the privilege" });
            }
        }
        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (ContentId.HasValue)
            {
                var result = ContentPrivilegesAPI.DeletePrivilege(ContentId.Value);
                if (result==ResultCodes.Succeed)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "Privilege wasn't found to be deleted" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Content id is required to delete the privilege" });
            }
        }
    }
}
