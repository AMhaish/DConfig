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
    public class CUDUsersAction : RegisterAction
    {
        public string NewUserName { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var user = await MembershipProvider.Instance.UserManager.FindByIdAsync(MembershipProvider.Instance.CurrentUserId);
            return Json(new {
                Email= user.Email,
                PhoneNumber= user.PhoneNumber,
                AccessFailedCount= user.AccessFailedCount,
                Id= user.Id,
                UserName= user.UserName
            });
        }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = UserName, Email = Email, EmailConfirmed = true , IsEnabled=true};
                var result = await MembershipProvider.Instance.UserManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    StringBuilder errors = new StringBuilder();
                    errors.AppendLine("Please correct the following errors:");
                    foreach (var error in result.Errors)
                    {
                        errors.AppendLine(error);
                    }
                    return Json(new { result = "false", message = errors.ToString() });
                }
            }
            else
            {
                StringBuilder errors = new StringBuilder();
                errors.AppendLine("Please correct the following errors:");
                foreach (var error in controller.ModelState.Values)
                {
                    foreach (var subError in error.Errors)
                    {
                        errors.AppendLine(subError.ErrorMessage);
                    }
                }
                return Json(new { result = "false", message = errors.ToString() });
            }
        }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(UserName) && controller.ModelState["Email"].Errors.Count <= 0)
            {
                var user = await MembershipProvider.Instance.UserManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    //UserManager.RemovePassword(user.Id);
                    //UserManager.AddPassword(user.Id, Password);
                    user.Email = Email;
                    if (!String.IsNullOrEmpty(NewUserName))
                    {
                        user.UserName = NewUserName;
                    }
                    else
                    {
                        user.UserName = UserName;
                    }
                    if (IsEnabled.HasValue)
                    {
                        user.IsEnabled = IsEnabled.Value;
                    }
                    IdentityResult x = await MembershipProvider.Instance.UserManager.UpdateAsync(user);
                    if (x.Succeeded)
                    {
                        //if (UserName != NewUserName)
                        //{
                        //    return await new LogOffAction().GetHandler(controller);
                        //}
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
            if (!String.IsNullOrEmpty(UserName))
            {
                var user = await MembershipProvider.Instance.UserManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    IdentityResult x = await MembershipProvider.Instance.UserManager.DeleteAsync(user);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "User can't be deleted" });
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
