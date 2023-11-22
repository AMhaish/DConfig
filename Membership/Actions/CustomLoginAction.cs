using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using SABFramework.PreDefinedModules.MembershipModule;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Membership.MembershipServices;
using Membership.Models;
using DConfigOS_Core.Layer2.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using Membership;

namespace Membership.Actions
{
    public class CustomLoginAction : DConfigOS_Core.Layer2.FormsServices.FormBaseAction
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string returnUrl { get; set; }
        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            var model = BuildCurrentFormModel();
            if (InitializeFormStateAndValidateIt(controller, model))
            {
                if (controller.ModelState.IsValid)
                {
                    var user = await MembershipProvider.Instance.UserManager.FindAsync(UserName, Password);
                    if (user != null)
                    {
                        if (user.IsEnabled == true)
                        {
                            if (await MembershipProvider.Instance.UserManager.IsEmailConfirmedAsync(user.Id) || true)
                            {
                                await MembershipProvider.Instance.SignIn(controller.HttpContext, user, RememberMe);
                                if (!String.IsNullOrEmpty(model.PageForm.SubmitRedirectUrl))
                                    return Redirect(model.PageForm.SubmitRedirectUrl);
                                else if (!String.IsNullOrEmpty(returnUrl))
                                    return Redirect(returnUrl);
                                else
                                    return Redirect("/");
                            }
                            else
                            {
                                model.ModelState.AddModelError("ConEmail", "You must have a confirmed email to login.");
                                SetFormValidaty(model, false);
                            }
                        }
                        else
                        {
                            model.ModelState.AddModelError("ActAcc", "Your account is not activated yet, or deactivated.");
                            SetFormValidaty(model, false);
                        }
                    }
                    else
                    {
                        user = await MembershipProvider.Instance.UserManager.FindByEmailAsync(UserName);
                        if (user != null)
                        {
                            if (user.IsEnabled == true)
                            {
                                if (await MembershipProvider.Instance.UserManager.CheckPasswordAsync(user, Password))
                                {
                                    if (await MembershipProvider.Instance.UserManager.IsEmailConfirmedAsync(user.Id))
                                    {
                                        await MembershipProvider.Instance.SignIn(controller.HttpContext, user, RememberMe);
                                        if (!String.IsNullOrEmpty(model.PageForm.SubmitRedirectUrl))
                                            return Redirect(model.PageForm.SubmitRedirectUrl);
                                        else if (!String.IsNullOrEmpty(returnUrl))
                                            return Redirect(returnUrl);
                                        else
                                            return Redirect("/");
                                    }
                                    else
                                    {
                                        model.ModelState.AddModelError("ConEmail", "You must have a confirmed email to login.");
                                        SetFormValidaty(model, false);
                                    }
                                }
                                else
                                {
                                    model.ModelState.AddModelError("Auth", "Authentication failed. Please recheck user name and password.");
                                    SetFormValidaty(model, false);
                                }
                            }
                            else
                            {
                                model.ModelState.AddModelError("ActAcc", "Your account is not activated yet, or deactivated.");
                            }                                SetFormValidaty(model, false);

                        }
                        else
                        {
                            model.ModelState.AddModelError("Auth", "Authentication failed. Please recheck user name and password.");
                            SetFormValidaty(model, false);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, ModelState> error in controller.ModelState)
                    {
                        foreach (var suberror in error.Value.Errors)
                        {
                            model.ModelState.AddModelError(error.Key, suberror.ErrorMessage);
                        }
                    }
                    model.ModelState.AddModelError("In", "Inputs provided is invalid, please correct the errors and try again.");
                    SetFormValidaty(model, false);
                }
            }
            return await new RenderContentAction() { Path = (PageUrl != null ? PageUrl.TrimStart('/') : null), PageFormId = model.PageForm.Id }.GetHandler(controller);
        }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return await new RenderContentAction() { Path = (PageUrl != null ? PageUrl.TrimStart('/') : null) }.GetHandler(controller);
        }
    }
}
