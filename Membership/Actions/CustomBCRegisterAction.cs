using System;
using System.Text;
using SABFramework.Core;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SABFramework.PreDefinedModules.MembershipModule;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.PreDefinedModules.MembershipModule.Actions;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Membership.Actions
{
    public class CustomBCRegisterAction : RegisterAction
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = UserName, Email = Email, EmailConfirmed = false, IsEnabled = true,  };
                var result = MembershipProvider.Instance.UserManager.Create(user, Password);
                if (result.Succeeded)
                {
                    if (!String.IsNullOrEmpty(RoleName))
                    {
                        IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(user.Id, RoleName);
                        if (!x.Succeeded)
                        {
                            IdentityResult xx = await MembershipProvider.Instance.UserManager.DeleteAsync(user);
                            return Json(new { result = "false", message = "Incorrect requested role name" });
                        }
                    }
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    var sendGridClient = new SendGridClient(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridAPIKey"]);
                    var sendGridMessage = new SendGridMessage();
                    sendGridMessage.Subject = "Confirm your email with us.";
                    sendGridMessage.SetFrom(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridFrom"], "BC No Reply");
                    sendGridMessage.AddTo(user.Email);
                    sendGridMessage.SetTemplateId(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridConfirmEmailTemplateId"]);
                    sendGridMessage.SetTemplateData(new
                    {
                        email = user.Email,
                        userId = user.Id,
                        name = FirstName + " " + LastName
                    });
                    try
                    {
                        var response = await sendGridClient.SendEmailAsync(sendGridMessage);
                        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                        {
                            return Json(new { result = "true" });
                        }
                        else
                        {
                            return Json(new { result = "false", message = "Process failed" });
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { result = "false", message = ex.Message });
                    }
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
                errors.AppendLine("Inputs provided is invalid, please correct the following errors:");
                foreach (var error in controller.ModelState.Values)
                {
                    foreach (var suberror in error.Errors)
                    {
                        errors.AppendLine(suberror.ErrorMessage);
                    }
                }
                return Json(new { result = "false", message = errors.ToString() });
            }
        }
    }
}
