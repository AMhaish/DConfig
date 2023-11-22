using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.Security.Claims;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class ExternalLoginAction : SABFramework.Core.SABAction
    {
        public string returnUrl { get; set; }
        public string Provider { get; set; }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            controller.Session["EnableOAuth"] = true;
            return UserDefinedResult(new MembershipProvider.ChallengeResult(Provider, controller.Url.Action("ExternalLoginCallback", "Membership", new { returnUrl = returnUrl })));
        }

        public async override Task<SABActionResult> GetHandler(Controller controller)
        {
            var loginInfo = await MembershipProvider.Instance.GetExternalLoginInfo(controller.HttpContext);
            if (loginInfo == null)
            {
                return Redirect("/?errorCode=" + SABFramework.Core.DataCore.ErrorCodes.InvalidExternalLoginInfo);
            }
            //Sign in the user with this external login provider if the user already has a login
            var result = await MembershipProvider.Instance.SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    if (String.IsNullOrEmpty(returnUrl))
                        return Redirect("/");
                    else
                        return Redirect(returnUrl);
                case SignInStatus.LockedOut:
                    throw new NotImplementedException();
                case SignInStatus.RequiresVerification:
                    throw new NotImplementedException();
                case SignInStatus.Failure:
                default:
                    break; // To continue OAuth registration
            }
            var existsUser = await MembershipProvider.Instance.UserManager.FindByEmailAsync(loginInfo.Email);
            if (existsUser != null)
            {
                await MembershipProvider.Instance.SignIn(controller.HttpContext, existsUser, true);
                return Redirect("/");
            }
            var user = new ApplicationUser()
            {
                UserName = loginInfo.DefaultUserName
            };
            ExtractExternalInfo(user, loginInfo);
            var result2 = MembershipProvider.Instance.UserManager.Create(user);
            if (result2.Succeeded)
            {
                result2 = MembershipProvider.Instance.UserManager.AddLogin(user.Id, loginInfo.Login);
                if (result2.Succeeded)
                {
                    await MembershipProvider.Instance.SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    //await MembershipProvider.Instance.SignIn(controller.HttpContext, user, isPersistent: false);
                    if (String.IsNullOrEmpty(returnUrl))
                        return Redirect("/");
                    else
                        return Redirect(returnUrl);
                }
                else
                {
                    return Redirect("/?errorCode=" + SABFramework.Core.DataCore.ErrorCodes.UserCouldntBeSignedIn);
                }
            }
            else
            {
                return Redirect("/?errorCode=" + SABFramework.Core.DataCore.ErrorCodes.UserCouldntBeCreated);
            }

        }

        public void ExtractExternalInfo(ApplicationUser User, Microsoft.AspNet.Identity.Owin.ExternalLoginInfo identity)
        {
            var emailClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null)
                User.Email = emailClaim.Value;
            //var countryClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Country);
            //var birthClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth);
            //var genderClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Gender);
            //var homePhoneClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.HomePhone);
            //var mobilePhoneClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone);
            //var nameClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            //var postalCodeClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PostalCode);
            //var sureNameClaim = identity.ExternalIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
        }
    }
}
