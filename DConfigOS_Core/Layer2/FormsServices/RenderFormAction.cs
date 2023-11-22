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

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class RenderFormAction : FormBaseAction
    {

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var currentDomain = controller.HttpContext.Request.Url.Host;
            if (currentDomain == "localhost")
            {
                currentDomain = SABFramework.Core.SABCoreEngine.Instance.Settings["Domain"];
            }
            if (DConfigRequestContext.Domains.ContainsKey(currentDomain))
            {
                DConfigRequestContext.Current.DomainId = DConfigRequestContext.Domains[currentDomain];
                DConfigRequestContext.Current.ContextId = DConfigRequestContext.Contexts[currentDomain];
            }
            var model = BuildCurrentFormModel();
            foreach(var state in model.ModelState)
            {
                controller.ModelState.Add(state);
            }
            switch (model.PageForm.Type)
            {
                case "Single Form":
                default:
                    return PartialView("~/Views/DConfigOS/Partials/SingleFormTemplate.cshtml", model);
                case "Multiple Sections Form":
                    return PartialView("~/Views/DConfigOS/Partials/MultipleSectionsFormTemplate.cshtml", model);
                case "Multiple Steps Form":
                    return PartialView("~/Views/DConfigOS/Partials/MultipleStepsFormTemplate.cshtml", model);
                case "Multiple Same Child":
                    return PartialView("~/Views/DConfigOS/Partials/MultipleSameChildTemplate.cshtml", model);
            }
        }

        public override async Task<SABActionResult> PostHandler(Controller controller)
        {
            return await GetHandler(controller);
        }
    }
}
