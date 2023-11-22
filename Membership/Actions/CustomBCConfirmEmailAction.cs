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
using SABFramework.PreDefinedModules.MembershipModule;

namespace Membership.Actions
{
    public class CustomBCConfirmEmailAction : SABFramework.Core.SABAction
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
                        if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return Redirect("https://app.bcrumbs.net/account/login");
                    }
                    else
                    {
                        return Redirect("https://app.bcrumbs.net/account/error?message=The%20user%20is%20already%20confirmed");
                    }
                }
                else
                {
                    return Redirect("https://app.bcrumbs.net/account/error?message=User%20could%20not%20be%20found%20to%20be%20confirmed%2C%20invitation%20might%20be%20expired");
                }
            }
            else
            {
                return Redirect("https://app.bcrumbs.net/account/error?message=User%20could%20not%20be%20found%20to%20be%20confirmed%2C%20invitation%20might%20be%20expired");
            }
        }
    }
}
