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

namespace Membership.Actions
{
    public class CustomRegisterAction : DConfigOS_Core.Layer2.FormsServices.FormBaseAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        public string returnUrl { get; set; }

        public string RoleName { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            CurrentFormModel = BuildCurrentFormModel();
            if (InitializeFormStateAndValidateIt(controller, CurrentFormModel))
            {
                if (controller.ModelState.IsValid)
                {
                    CreateFormInstancesForFormModels(controller, CurrentFormModel);
                    ProcessFormSubmitEvents(controller, CurrentFormModel);
                    ClearFormFromSession(CurrentFormModel);
                    var needConfirmation = SABCoreEngine.Instance.Settings.ContainsKey("EnableNormalSignUpEmailsNotifications") && SABFramework.Core.SABCoreEngine.Instance.Settings["EnableNormalSignUpEmailsNotifications"] == "True";
                    var user = new ApplicationUser() { UserName = UserName, Email = Email, EmailConfirmed = !needConfirmation, IsEnabled = (String.IsNullOrEmpty(RoleName) ? true : false) };
                    var result = MembershipProvider.Instance.UserManager.Create(user, Password);
                    if (result.Succeeded)
                    {
                        if (!String.IsNullOrEmpty(RoleName))
                        {
                            IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(user.Id, RoleName);
                            if (x.Succeeded)
                            {
                                if (needConfirmation)
                                {
                                    try
                                    {
                                        System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress(SABFramework.Core.SABCoreEngine.Instance.Settings["RegistrationEmail"]), new System.Net.Mail.MailAddress(user.Email));
                                        m.Subject = "Email confirmation";
                                        m.Body = string.Format("Dear {0} <br/> Thank you for your registration, however the registration process needs further checking from our administrative department.", user.UserName);
                                        m.IsBodyHtml = true;
                                        var smtp_host = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Host);
                                        var smtp_port = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Port);
                                        var smtp_username = websiteSettingsAPI.Get(WebsiteSetting.SMTP_UserName);
                                        var smtp_password = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Password);
                                        var smtp_from = websiteSettingsAPI.Get(WebsiteSetting.SMTP_From);
                                        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(smtp_host, Convert.ToInt32(smtp_port));
                                        smtp.Credentials = new NetworkCredential(smtp_username, smtp_password);
                                        ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                                        smtp.Send(m);
                                    }
                                    catch (Exception ex)
                                    {
                                        SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in sending registration confirmation email.", ex);
                                    }
                                }
                                if (!String.IsNullOrEmpty(CurrentFormModel.PageForm.SubmitRedirectUrl))
                                    return Redirect(CurrentFormModel.PageForm.SubmitRedirectUrl);
                                else if (!String.IsNullOrEmpty(returnUrl))
                                    return Redirect(returnUrl);
                                else
                                    return Redirect("/");
                            }
                            else
                            {
                                IdentityResult xx = await MembershipProvider.Instance.UserManager.DeleteAsync(user);
                                CurrentFormModel.ModelState.AddModelError("R_Role", "Incorrect requested role name");
                                SetFormValidaty(CurrentFormModel, false);
                            }
                        }
                        else
                        {
                            if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey("EnableNormalSignUpEmailsNotifications") && SABFramework.Core.SABCoreEngine.Instance.Settings["EnableNormalSignUpEmailsNotifications"] == "True")
                            {
                                try
                                {
                                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress(SABFramework.Core.SABCoreEngine.Instance.Settings["RegistrationEmail"]), new System.Net.Mail.MailAddress(user.Email));
                                    m.Subject = "Email confirmation";
                                    m.Body = string.Format("Dear {0} <br/> Thank you for your registration, please click on the below link to complete your registration: <br/><a href =\"{1}\" title =\"User Email Confirm\">{1}</a>",
                                       user.UserName, controller.Url.Action("ConfirmEmail", "Membership", new { Token = user.Id, Email = user.Email, returnUrl = (!String.IsNullOrEmpty(returnUrl) ? returnUrl : "/") }, controller.Request.Url.Scheme));
                                    m.IsBodyHtml = true;
                                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                                    ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
                                    smtp.Send(m);
                                }
                                catch (Exception ex)
                                {
                                    SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in sending registration confirmation email.", ex);
                                }
                            }
                        }
                        //await MembershipProvider.Instance.SignIn(controller.HttpContext, user, isPersistent: false);
                        if (!String.IsNullOrEmpty(CurrentFormModel.PageForm.SubmitRedirectUrl))
                            return Redirect(CurrentFormModel.PageForm.SubmitRedirectUrl);
                        else if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return Redirect("/");

                    }
                    else
                    {
                        StringBuilder errors = new StringBuilder();
                        errors.AppendLine("Please correct the following errors:");
                        foreach (var error in result.Errors)
                        {
                            errors.AppendLine(error);
                        }
                        CurrentFormModel.ModelState.AddModelError("", errors.ToString());
                        SetFormValidaty(CurrentFormModel, false);
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
                    CurrentFormModel.ModelState.AddModelError("R_In", errors.ToString());
                    SetFormValidaty(CurrentFormModel, false);
                }
            }
            return await new RenderContentAction() { Path = (PageUrl != null ? PageUrl.TrimStart('/') : null), PageFormId = CurrentFormModel.PageForm.Id }.GetHandler(controller);
        }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return await new RenderContentAction() { Path = (PageUrl != null ? PageUrl.TrimStart('/') : null) }.GetHandler(controller);
        }
    }
}
