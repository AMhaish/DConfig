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
using System.Net;
using SABFramework.Providers;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class RegisterAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IEmailProvider emailProvider { protected get; set; }


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
        [RegularExpression("^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$")]
        public string Email { get; set; }

        public string RoleName { get; set; }

        public bool? IsEnabled { get; set; }

        public Company Company { get; set; }

        public int? ContextCompanyId { get; set; }

        protected ApplicationUser addedUser { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var needConfirmation = SABCoreEngine.Instance.Settings.ContainsKey("EnableNormalSignUpEmailsNotifications") && SABFramework.Core.SABCoreEngine.Instance.Settings["EnableNormalSignUpEmailsNotifications"] == "True";
                var user = new ApplicationUser() { UserName = UserName, Email = Email, EmailConfirmed = !needConfirmation, IsEnabled = (String.IsNullOrEmpty(RoleName) ? true : false) };
                var result = MembershipProvider.Instance.UserManager.Create(user, Password);
                if (result.Succeeded)
                {
                    MembershipDBContext dbContext = new MembershipDBContext();
                    addedUser = dbContext.Users.Where(m => m.Email == Email).FirstOrDefault();
                    if (Company != null)
                    {
                        if (addedUser != null)
                        {
                            addedUser.Companies = new List<Company>();
                            Company.CreateDate = DateTime.Now;
                            addedUser.Companies.Add(Company);
                            dbContext.SaveChanges();
                        }
                    }
                    if (!String.IsNullOrEmpty(RoleName))
                    {
                        IdentityResult x = await MembershipProvider.Instance.UserManager.AddToRoleAsync(user.Id, RoleName);
                        if (x.Succeeded)
                        {
                            if (needConfirmation==true)
                            {
                                try
                                {
                                    IdentityMessage m = new IdentityMessage();
                                    m.Destination = user.Email;
                                    m.Subject = "Email confirmation";
                                    m.Body = string.Format("Dear {0} <br/> Thank you for your registration, however the registration process needs further checking from our administrative department.", user.UserName);
                                    if (ContextCompanyId.HasValue)
                                    {
                                        await emailProvider.SendAsync(m, ContextCompanyId.Value);
                                    }
                                    else
                                    {
                                        await emailProvider.SendAsync(m);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in sending registration confirmation email.", ex);
                                }
                            }
                            return Json(new { result = "true" , userId = addedUser.Id});
                        }
                        else
                        {
                            IdentityResult xx = await MembershipProvider.Instance.UserManager.DeleteAsync(user);
                            return Json(new { result = "false", message = "Incorrect requested role name" });
                        }
                    }
                    else
                    {
                        if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey("EnableNormalSignUpEmailsNotifications") && SABFramework.Core.SABCoreEngine.Instance.Settings["EnableNormalSignUpEmailsNotifications"] == "True")
                        {
                            //For normal users
                            try
                            {
                                IdentityMessage m = new IdentityMessage();
                                m.Destination = user.Email;
                                m.Subject = "Email confirmation";
                                m.Body = string.Format("Dear {0} <br/> Thank you for your registration, please click on the below link to complete your registration: <br/><a href =\"{1}\" title =\"User Email Confirm\">{1}</a>",
                                    user.UserName, controller.Url.Action("ConfirmEmail", "Membership", new { Token = user.Id, Email = user.Email }, controller.Request.Url.Scheme));
                                if (ContextCompanyId.HasValue)
                                {
                                    await emailProvider.SendAsync(m, ContextCompanyId.Value);
                                }
                                else
                                {
                                    await emailProvider.SendAsync(m);
                                }
                            }
                            catch (Exception ex)
                            {
                                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in sending registration confirmation email.", ex);
                            }
                        }
                    }
                    //await MembershipProvider.Instance.SignIn(controller.HttpContext, user, isPersistent: false);
                    return Json(new { result = "true", userId = addedUser.Id });
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
