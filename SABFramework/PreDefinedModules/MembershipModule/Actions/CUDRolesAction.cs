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
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class CUDRolesAction : SABFramework.Core.SABAction
    {
        [Required]
        public string RoleName { get; set; }

        public string NewRoleName { get; set; }
        protected RoleManager<IdentityRole> RoleManager { get; private set; }

        public CUDRolesAction()
        {
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new MembershipDBContext()));
        }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var role = new IdentityRole() { Name= RoleName };
                var result = await RoleManager.CreateAsync(role);
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
            return null;
        }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(RoleName))
            {
                var role = await RoleManager.FindByNameAsync(RoleName);
                if (role != null)
                {
                    //UserManager.RemovePassword(user.Id);
                    //UserManager.AddPassword(user.Id, Password);
                    role.Name = NewRoleName;
                    IdentityResult x = await RoleManager.UpdateAsync(role);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "Role can't be updated" });
                    }
                }
                else
                {
                    return Json(new { result = "false", message = "Role with name (" + RoleName + ") hasn't found in the database." });
                }
            }
            return null;
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(RoleName))
            {
                var role = await RoleManager.FindByNameAsync(RoleName);
                if (role != null)
                {
                    IdentityResult x = await RoleManager.DeleteAsync(role);
                    if (x.Succeeded)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        return Json(new { result = "false", message = "Role can't be deleted" });
                    }
                }
                else
                {
                    return Json(new { result = "false", message = "Role with name (" + RoleName + ") hasn't found in the database." });
                }
            }
            return null;
        }
    }
}
