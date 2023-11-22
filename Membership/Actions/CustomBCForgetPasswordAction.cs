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
using SABFramework.PreDefinedModules.MembershipModule;
using System.Security.Claims;
using Membership.MembershipServices;
using Membership.Models;
using DConfigOS_Core.Layer2.WebsiteContentServices;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using DConfigOS_Core.Repositories.Utilities;
using System.Net;
using Ninject;
using SABFramework.Providers;
using static SABFramework.Providers.EmailProvider;
using DConfigOS_Core.Providers.HttpContextProviders;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Membership.Actions
{
    public class CustomBCForgetPasswordAction : SABFramework.Core.SABAction
    {
        [Required]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
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
                    string encodedCode = HttpUtility.UrlEncode(code);
                    var sendGridClient = new SendGridClient(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridAPIKey"]);
                    var sendGridMessage = new SendGridMessage();
                    sendGridMessage.Subject = "You've requested a password reset";
                    sendGridMessage.SetFrom(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridFrom"], "BC No Reply");
                    sendGridMessage.AddTo(user.Email);
                    sendGridMessage.SetTemplateId(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridResetPasswordTemplateId"]);
                    sendGridMessage.SetTemplateData(new
                    {
                        code = encodedCode,
                        userId = user.Id
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
                    } catch (Exception ex)
                    {
                        return Json(new { result = "false", message = ex.Message });
                    }
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
