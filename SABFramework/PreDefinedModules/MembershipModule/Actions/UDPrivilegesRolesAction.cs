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
    public class UDPrivilegesRolesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPrivilegeAPI privilegeAPI { get; set; }

        public string RoleName { get; set; }
        public int? PrivilegeId { get; set; }
        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (PrivilegeId.HasValue && !String.IsNullOrEmpty(RoleName))
            {
                var result = privilegeAPI.AddPrivilegeToRole(PrivilegeId.Value, RoleName);
                if (result)
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
            if (PrivilegeId.HasValue && !String.IsNullOrEmpty(RoleName))
            {
                var result = privilegeAPI.RemovePrivilegeFromRole(PrivilegeId.Value, RoleName);
                if (result)
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
