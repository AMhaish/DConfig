using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetTemplateViewTypeFieldsAction : UserActionsBase
    {
        [Inject]
        public ITemplatesAPI templatesAPI { get; set; }
        [Inject]
        public IViewFieldsAPI viewFieldsAPI { get; set; }

        public int Id { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var template = templatesAPI.GetTemplate(Id);
            if (template.ViewTypeId.HasValue)
            {
                var fields = viewFieldsAPI.GetViewTypeFields(template.ViewTypeId.Value);
                return Json(fields);
            }
            else
            {
                return Json(new { });
            }
        }
    }
}
