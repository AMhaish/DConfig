using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Providers.HttpContextProviders;
using System.Net.Mail;
using Newtonsoft.Json;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class FormSubmitAction : FormBaseAction
    {
        [JsonIgnore]
        public bool? AjaxCall { get; set; }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            var model = BuildCurrentFormModel();
            CurrentFormModel = model;
            var targetForm = formsAPI.GetForm(Id);
            if (!targetForm.ReCapatchaEnabled.HasValue || !targetForm.ReCapatchaEnabled.Value || await ValidateRecapatcha(controller, targetForm.Name))
            {
                if (InitializeFormStateAndValidateIt(controller, model))
                {
                    CreateFormInstancesForFormModels(controller, model);
                    await ProcessFormSubmitEvents(controller, model);
                    ClearFormFromSession(model);
                    if (AjaxCall.HasValue && AjaxCall.Value)
                    {
                        return Json(new { result = "true" });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.PageForm.SubmitRedirectUrl))
                        {
                            return Redirect(model.PageForm.SubmitRedirectUrl);
                        }
                        else
                        {
                            return Redirect("/");
                        }
                    }

                }
            }
            if (AjaxCall.HasValue && AjaxCall.Value)
                return Json(new { result = "false", message = "Exception happened, please contact support team." });
            else
                return await new WebsiteContentServices.RenderContentAction() { Path = PageUrl.TrimStart('/'), PageFormId = model.PageForm.Id }.GetHandler(controller);
        }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return await new WebsiteContentServices.RenderContentAction() { Path = PageUrl.TrimStart('/') }.GetHandler(controller);
        }
    }
}
