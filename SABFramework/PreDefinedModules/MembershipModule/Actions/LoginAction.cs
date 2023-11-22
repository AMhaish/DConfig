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


namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class LoginAction : SABFramework.Core.SABAction
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
            if (controller.ModelState.IsValid)
            {
                var user = MembershipProvider.Instance.UserManager.Find(UserName, Password);
                if (user != null)
                {
                    if (user.IsEnabled == true)
                    {
                        if (await MembershipProvider.Instance.UserManager.IsEmailConfirmedAsync(user.Id))
                        {
                            await MembershipProvider.Instance.SignIn(controller.HttpContext, user, RememberMe);
                            return Json(new { result = "true", returnUrl = returnUrl });
                        }
                        else
                        {
                            return Json(new { result = "false", message = "You must have a confirmed email to login." });
                        }
                    }
                    else
                    {
                        return Json(new { result = "false", message = "Your account is not activated yet, or deactivated." });
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
                                    //var resultedUser = await MembershipProvider.Instance.UserManager.FindAsync(user.UserName, Password);
                                    if (user != null)
                                    {
                                        await MembershipProvider.Instance.SignIn(controller.HttpContext, user, RememberMe);
                                        return Json(new { result = "true", returnUrl = returnUrl });
                                    }
                                    else
                                    {
                                        return Json(new { result = "false", message = "Authentication failed. Please recheck user name and password." });
                                    }
                                }
                                else
                                {
                                    return Json(new { result = "false", message = "You must have a confirmed email to login." });
                                }
                            }
                            else
                            {
                                return Json(new { result = "false", message = "Authentication failed. Please recheck user name and password." });
                            }
                        }
                        else
                        {
                            return Json(new { result = "false", message = "Your account is not activated yet, or deactivated." });
                        }
                    }
                    else
                    {
                        return Json(new { result = "false", message = "Authentication failed. Please recheck user name and password." });
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
