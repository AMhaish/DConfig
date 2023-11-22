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
using SendGrid;
using SendGrid.Helpers.Mail;
using Ninject;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Actions;
using SABFramework.PreDefinedModules.MembershipModule;

namespace Membership.Actions
{
    public class CustomBCInviteUserAction : RegisterAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            int? companyId = MembershipProvider.Instance.ContextCompanyId;
            var targetCompany = companiesAPI.GetCompany(companyId.Value);
            if (!String.IsNullOrEmpty(Email) && companyId.HasValue)
            {
                var existsUser = await MembershipProvider.Instance.UserManager.FindByEmailAsync(Email);
                if (existsUser == null)
                {
                    // User will be created with temp password, and an email will be sent to the user to reset the password
                    var user = new ApplicationUser() { UserName = Email, Email = Email, EmailConfirmed = true, IsEnabled = true };
                    var result = await MembershipProvider.Instance.UserManager.CreateAsync(user, "DEFAULT_PASSWORD_FOR_INVITATION987_");

                    if (!String.IsNullOrEmpty(RoleName))
                    {
                        IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(user.Id, RoleName);
                        if (!x.Succeeded)
                        {
                            IdentityResult xx = await MembershipProvider.Instance.UserManager.DeleteAsync(user);
                            return Json(new { result = "false", message = "Incorrect requested role name" });
                        }
                    }

                    if (result.Succeeded)
                    {
                        var resultOfAddingUserToCompany = companiesAPI.AddUsersToCompany(companyId.Value, new List<string>() { user.Id });
                        if (resultOfAddingUserToCompany && targetCompany != null)
                        {
                            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                            // Send an email with this link
                            string code = MembershipProvider.Instance.UserManager.GeneratePasswordResetToken(user.Id);
                            string encodedCode = HttpUtility.UrlEncode(code);
                            var sendGridClient = new SendGridClient(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridAPIKey"]);
                            var sendGridMessage = new SendGridMessage();
                            sendGridMessage.Subject = "You've been invited to join \"" + targetCompany.Name + "\" team";
                            sendGridMessage.SetFrom(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridFrom"], "BC No Reply");
                            sendGridMessage.AddTo(user.Email);
                            sendGridMessage.SetTemplateId(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridInvitationTemplateId"]);
                            sendGridMessage.SetTemplateData(new
                            {
                                team = targetCompany.Name,
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
                            }
                            catch (Exception ex)
                            {
                                return Json(new { result = "false", message = ex.Message });
                            }

                        }

                        return Json(new { result = "false" });
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
                    var result = companiesAPI.AddUsersToCompany(companyId.Value, new List<string>() { existsUser.Id });
                    if (!String.IsNullOrEmpty(RoleName))
                    {
                        var isAlreadyInTheRole = MembershipProvider.Instance.UserManager.IsInRole(existsUser.Id, RoleName);
                        if (!isAlreadyInTheRole)
                        {
                            IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(existsUser.Id, RoleName);
                            if (!x.Succeeded)
                            {
                                return Json(new { result = "false", message = "Incorrect requested role name" });
                            }
                        }
                    }

                    if (result == true)
                    {
                        var sendGridClient = new SendGridClient(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridAPIKey"]);
                        var sendGridMessage = new SendGridMessage();
                        sendGridMessage.Subject = "You've been invited to join \"" + targetCompany.Name + "\" team";
                        sendGridMessage.SetFrom(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridFrom"], "BC No Reply");
                        sendGridMessage.AddTo(existsUser.Email);
                        sendGridMessage.SetTemplateId(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridInvitationPreExistsTemplateId"]);
                        sendGridMessage.SetTemplateData(new
                        {
                            team = targetCompany.Name
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
                        return Json(new { result = "false", message = "Couldn't find the company to be updated, or couldn't find the users you want to add to the company" });
                    }
                }
            }
            else
            {
                StringBuilder errors = new StringBuilder();
                errors.AppendLine("Please correct the following errors:");
                foreach (var error in controller.ModelState.Values)
                {
                    foreach (var subError in error.Errors)
                    {
                        errors.AppendLine(subError.ErrorMessage);
                    }
                }
                return Json(new { result = "false", message = errors.ToString() });
            }
        }
    }
}
