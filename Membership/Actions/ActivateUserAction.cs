using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule;
using Microsoft.AspNet.Identity;

namespace Membership.Actions
{
    public class ActivateUserAction : SABFramework.Core.SABAction
    {
        [Required]
        public string Id { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                ApplicationUser user = MembershipProvider.Instance.UserManager.FindByName(Id);
                if (user != null)
                {
                    if (!user.IsEnabled)
                    {
                        user.IsEnabled = true;
                        await MembershipProvider.Instance.UserManager.UpdateAsync(user);
                        return View(new { result = "true", message = "User have been activated successfully" });
                    }
                    else
                    {
                        return View(new { result = "false", message = "User have been already activated" });
                    }
                }
                else
                {
                    return View(new { result = "false", message = "User couldn't be found" });
                }
            }
            else
            {
                return View(new { result = "false", message = "Id is required for activating" });
            }
        }

    }
}
