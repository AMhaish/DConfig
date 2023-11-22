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

namespace Membership.Actions
{
    public class CustomForgetPasswordAction : DConfigOS_Core.Layer2.FormsServices.FormBaseAction
    {
        [Required]
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$", ErrorMessage = "Email is invalid")]
        public string Email { get; set; }
        public string returnUrl { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            CurrentFormModel = BuildCurrentFormModel();
            if (InitializeFormStateAndValidateIt(controller, CurrentFormModel))
            {
                if (controller.ModelState.IsValid)
                {
                    CreateFormInstancesForFormModels(controller, CurrentFormModel);
                    await ProcessFormSubmitEvents(controller, CurrentFormModel);
                    ClearFormFromSession(CurrentFormModel);
                    var user = await MembershipProvider.Instance.UserManager.FindByEmailAsync(Email);
                    if (user == null)
                    {
                        CurrentFormModel.ModelState.AddModelError("RST_User", "A user for this email couldn't be found.");
                        SetFormValidaty(CurrentFormModel, false);
                    }
                    else if (!(await MembershipProvider.Instance.UserManager.IsEmailConfirmedAsync(user.Id)))
                    {
                        CurrentFormModel.ModelState.AddModelError("RST_ConfirmEmail", "Email is not confirmed yet");
                        SetFormValidaty(CurrentFormModel, false);
                    }
                    else
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        string code = MembershipProvider.Instance.UserManager.GeneratePasswordResetToken(user.Id);
                        var callbackUrl = controller.Url.Action("ResetPasswordThroughEmail", "Membership", new { Token = code, UserId = user.Id }, controller.Request.Url.Scheme);
                        IdentityMessage message = new IdentityMessage();
                        message.Destination = user.Email;
                        message.Body = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";
                        message.Subject = "Reset Password";
                        int? contextId = DConfigRequestContext.Current.ContextId.HasValue ? DConfigRequestContext.Current.ContextId : (MembershipProvider.Instance.ContextCompanyId.HasValue ? MembershipProvider.Instance.ContextCompanyId : null);
                        if (contextId.HasValue)
                        {
                            await emailProvider.SendAsync(message, DConfigRequestContext.Current.ContextId.Value);
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
                            errors.AppendLine("No context company has been set!");
                            CurrentFormModel.ModelState.AddModelError("", errors.ToString());
                            SetFormValidaty(CurrentFormModel, false);
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
