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
using DConfigOS_Core.Repositories.Utilities;
using System.Net;

namespace Membership.Actions
{
    public class CustomResetPasswordAction : DConfigOS_Core.Layer2.FormsServices.FormBaseAction
    {
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
                    Token = System.Web.HttpUtility.UrlDecode(Token).Replace(" ","+");
                    var result = await MembershipProvider.Instance.UserManager.ResetPasswordAsync(UserId, Token, Password);
                    if (result.Succeeded)
                    {
                        if (!String.IsNullOrEmpty(CurrentFormModel.PageForm.SubmitRedirectUrl))
                            return Redirect(CurrentFormModel.PageForm.SubmitRedirectUrl);
                        else if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);
                        else
                            return Redirect("/");
                    }
                    else
                    {
                        CurrentFormModel.ModelState.AddModelError("RST_Password", "User password couldn't be changed, invalid token. Please try again to apply to forget password form to get new token.");
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
                    CurrentFormModel.ModelState.AddModelError("RST_In", errors.ToString());
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
