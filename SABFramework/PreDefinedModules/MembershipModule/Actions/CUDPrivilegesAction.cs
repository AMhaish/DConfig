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
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class CUDPrivilegesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPrivilegeAPI privilegesAPI { get; set; }

        public int? Id { get; set; }
        public string PrvController { get; set; }
        public string PrvAction { get; set; }
        public string RequestType { get; set; }
        public bool NeedAuthentication { get; set; }
        public bool NeedAuthorization { get; set; }
        public bool RequireHttps { get; set; }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(PrvController))
            {
                var p = new Privilege() { Controller = PrvController, Action = PrvAction, RequestType = RequestType, NeedAuthentication = NeedAuthentication, NeedAuthorization = NeedAuthorization, RequireHttps=RequireHttps };
                var result = privilegesAPI.CreatePrivilege(p);
                var obj = privilegesAPI.GetPrivilege(p.Id);
                if (result)
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
                return Json(new { result = "false", message = "Controller name is required to create privilege" });
            }
        }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (Id.HasValue)
            {
                var p = new Privilege() { Controller = PrvController, Action = PrvAction, RequestType = RequestType, Id = Id.Value, NeedAuthentication = NeedAuthentication, NeedAuthorization = NeedAuthorization, RequireHttps = RequireHttps };
                var result = privilegesAPI.UpdatePrivilege(p);
                var obj = privilegesAPI.GetPrivilege(p.Id);
                if (result)
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
                return Json(new { result = "false", message = "Id is required to update the privilege" });
            }
        }
        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (Id.HasValue)
            {
                var result = privilegesAPI.DeletePrivilege(Id.Value);
                if (result)
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
                return Json(new { result = "false", message = "Id is required to delete the privilege" });
            }
        }
    }
}
