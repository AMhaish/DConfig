using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using DConfigOS_Core.Providers.HttpContextProviders;
using Newtonsoft.Json;
namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class RenderEditableContentAction : SABFramework.Core.SABAction
    {
        public string Path { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            controller.ViewBag.EditingMode = true;
            return await new WebsiteContentServices.RenderContentAction() { Path = Path }.GetHandler(controller);
        }
    }
}
