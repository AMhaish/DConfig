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

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class UDUsersRolesAction : RegisterAction
    {
        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(RoleName))
            {
                var user = await MembershipProvider.Instance.UserManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(user.Id, RoleName);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "User can't be updated" });
                    }
                }
                else
                {
                    return Json(new { result = "false", message = "User with name (" + UserName + ") hasn't found in the database." });
                }
            }
            return null;
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(RoleName))
            {
                var user = await MembershipProvider.Instance.UserManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    IdentityResult x = await MembershipProvider.Instance.UserManager.RemoveFromRoleAsync(user.Id, RoleName);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "User can't be removed from " + RoleName + " Role" });
                    }
                }
                else
                {
                    return Json(new { result = "false", message = "User with name (" + UserName + ") hasn't found in the database." });
                }
            }
            return null;
        }
    }
}
