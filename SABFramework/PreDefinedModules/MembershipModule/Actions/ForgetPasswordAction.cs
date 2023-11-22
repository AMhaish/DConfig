using System;
using SABFramework.Core;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SABFramework.Providers;
using Ninject;
using static SABFramework.Providers.EmailProvider;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class ForgetPasswordAction : RegisterAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { protected get; set; }

        public string CustomResetPasswordUrl { get; set; }

        public override async Task<SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(Email))
            {
                var user = await MembershipProvider.Instance.UserManager.FindByEmailAsync(Email);
                if (user == null)
                {

                    return Json(new { result = "false", message = "A user for this email couldn't be found." });
                }
                else if (!(await MembershipProvider.Instance.UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return Json(new { result = "false", message = "Email is not confirmed yet" });
                }
                else
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = MembershipProvider.Instance.UserManager.GeneratePasswordResetToken(user.Id);
                    code = System.Web.HttpUtility.UrlEncode(code);
                    string callbackUrl;
                    if (!String.IsNullOrEmpty(CustomResetPasswordUrl))
                    {
                        callbackUrl = CustomResetPasswordUrl + "?Token=" + code + "&UserId=" + user.Id;
                    }
                    else
                    {
                        callbackUrl = controller.Url.Action("ResetPasswordThroughEmail", "Membership", new { Token = code, UserId = user.Id }, controller.Request.Url.Scheme);
                    }
                    try { 
                        if (ContextCompanyId.HasValue)
                        {
                            SABIdentityMessage message = new SABIdentityMessage();
                            message.Destination = user.Email;
                            message.Model = callbackUrl;
                            message.View = websiteSettingsAPI.Get(WebsiteSetting.General_ForgetpasswordEmailTemplate);
                            message.Subject = "Reset Password";
                            await emailProvider.SendAsync(message, ContextCompanyId.Value);
                        }
                        else
                        {
                            IdentityMessage message = new IdentityMessage();
                            message.Destination = user.Email;
                            message.Body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";
                            message.Subject = "Reset Password";
                            await emailProvider.SendAsync(message);
                        }
                    }
                    catch(Exception ex)
                    {
                        return Json(new { result = "false", message = ex.Message });
                    }
                    return Json(new { result = "true" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Error: Email is empty." });
            }
        }

    }
}
