using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using System.Web.Mvc;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Providers.HttpContextProviders;
using Ninject;

namespace FormsManager.Actions
{
    public class GeneralFormSubmitAction : DConfigOS_Core.Layer2.FormsServices.FormBaseAction
    {
        public Guid FormId { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var form = formsAPI.GetFormByUniqueParam(FormId);
            Id = form.Id;
            var model = BuildCurrentFormModel();
            CurrentFormModel = model;
            foreach (var state in model.ModelState)
            {
                controller.ModelState.Add(state);
            }
            model.PageForm.CustomSubmitPath = "/DConfig/FormsManager/SubmitForm?FormId=" + FormId;
            controller.ViewBag.FormName = form.Name;
            switch (model.PageForm.Type)
            {
                case "Single Form":
                default:
                    return View("~/Views/DConfigOS/Partials/SingleFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml" ,model);
                case "Multiple Sections Form":
                    return View("~/Views/DConfigOS/Partials/MultipleSectionsFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml",model);
                case "Multiple Steps Form":
                    return View("~/Views/DConfigOS/Partials/MultipleStepsFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml",model);
                case "Multiple Same Child":
                    return View("~/Views/DConfigOS/Partials/MultipleSameChildTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml", model);
            }
        }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            var form = formsAPI.GetFormByUniqueParam(FormId);
            Id = form.Id;
            var model = BuildCurrentFormModel();
            model.PageForm.CustomSubmitPath = "/DConfig/FormsManager/SubmitForm?FormId=" + FormId;
            controller.ViewBag.FormName = model.PageForm.Name;
            if (InitializeFormStateAndValidateIt(controller,model))
            {
                CreateFormInstancesForFormModels(controller, model);
                await ProcessFormSubmitEvents(controller, model);
                ClearFormFromSession(model);
                return View("~/Views/AppsViews/FormsManager/GeneralFormSubmitted.cshtml");

            }
            switch (model.PageForm.Type)
            {
                case "Single Form":
                default:
                    return View("~/Views/DConfigOS/Partials/SingleFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml", model);
                case "Multiple Sections Form":
                    return View("~/Views/DConfigOS/Partials/MultipleSectionsFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml", model);
                case "Multiple Steps Form":
                    return View("~/Views/DConfigOS/Partials/MultipleStepsFormTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml", model);
                case "Multiple Same Child":
                    return View("~/Views/DConfigOS/Partials/MultipleSameChildTemplate.cshtml", "~/Views/AppsViews/FormsManager/_GeneralFormSubmitLayout.cshtml", model);
            }
        }

    }
}
