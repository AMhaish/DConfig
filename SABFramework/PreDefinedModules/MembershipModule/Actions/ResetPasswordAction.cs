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
    public class ResetPasswordAction : RegisterAction
    {
        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(Password) && Password==ConfirmPassword)
            {
                var user = await MembershipProvider.Instance.UserManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    IdentityResult x = await MembershipProvider.Instance.UserManager.RemovePasswordAsync(user.Id);
                    IdentityResult xx = await MembershipProvider.Instance.UserManager.AddPasswordAsync(user.Id,Password);
                    if (x.Succeeded && xx.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        StringBuilder errors = new StringBuilder();
                        errors.AppendLine("Please correct the following errors:");
                        foreach (var error in x.Errors)
                        {
                            errors.AppendLine(error);
                        }
                        foreach (var error in xx.Errors)
                        {
                            errors.AppendLine(error);
                        }
                        return Json(new { result = "false", message = errors.ToString() });
                    }
                }
                else
                {
                    return Json(new { result = "false", message = "" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Error: Password is empty or the password and the confirm password aren't identical." });
            }
        }

    }
}
