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
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class LogOffAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            MembershipProvider.Instance.SignOut(controller.HttpContext);
            var logoutAction = websiteSettingsAPI.Get("General_LogoutAction");
            if (!String.IsNullOrEmpty(logoutAction))
            {
                switch (logoutAction)
                {
                    case "DomainRoot":
                        return Redirect("/");
                    case "CustomRedirect":
                        var logoutRedirect = websiteSettingsAPI.Get("General_LogoutRedirect");
                        if (!String.IsNullOrEmpty(logoutRedirect))
                            return Redirect(logoutRedirect);
                        else
                            return Redirect("/");
                    case "LoginPage":
                        var loginPage = websiteSettingsAPI.Get("General_LoginUrl");
                        if (!String.IsNullOrEmpty(loginPage))
                            return Redirect(loginPage);
                        else
                            return Redirect("/");
                    default:
                        return Redirect("/");
                }
            }
            else
            {
                return Redirect("/");
            }
        }
    }
}
