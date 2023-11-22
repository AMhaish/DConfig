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
    public class ResetPasswordThroughEmailAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string returnUrl { get; set; }


        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            if (controller.Request.Url.Host == "dconfig.com" || controller.Request.Url.Host.StartsWith("localhost"))
            {
                return Redirect("/DConfig/Account#!/Resetpassword?Token=" + Token + "&UserId=" + UserId);
            }
            else
            {
                var resetPasswordPath = websiteSettingsAPI.Get("General_ResetPasswordPath");
                if (!String.IsNullOrEmpty(resetPasswordPath))
                {
                    return Redirect(resetPasswordPath + "?Token=" + Token + "&UserId=" + UserId);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = await MembershipProvider.Instance.UserManager.ResetPasswordAsync(UserId, Token, Password);
                if (result.Succeeded)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "User password couldn't be changed, maybe you are using an expired link. Please try again to repeat forget password process." });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Error: Password is empty or the password and the confirm password aren't identical." });
            }
        }

    }
}
