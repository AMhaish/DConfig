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

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class ConfirmEmailAction : SABFramework.Core.SABAction
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string returnUrl { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            ApplicationUser user = MembershipProvider.Instance.UserManager.FindById(Token);
            if (user != null)
            {
                if (user.Email == Email)
                {
                    if (!user.EmailConfirmed)
                    {
                        user.EmailConfirmed = true;
                        await MembershipProvider.Instance.UserManager.UpdateAsync(user);
                        await MembershipProvider.Instance.SignIn(controller.HttpContext, user, isPersistent: false);
                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return Redirect("/");
                    }
                    else
                    {
                        return Redirect(SABFramework.Core.SABCoreEngine.Instance.Settings["LoginUrl"] + "?message=The%20user%20is%20already%20confirmed");
                    }
                }
                else
                {
                    return Str("User couldn't be found to be confirmed");
                }
            }
            else
            {
                return Str("User couldn't be fond to be confirmed");
            }
        }
    }
}
