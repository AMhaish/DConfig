using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class GetFormSubmitEventsTreeAction : UserActionsBase
    {
        [Inject]
        public IFormsAPI formsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var forms = formsAPI.GetForms().Select(a => new TreeNodeModel()
            {
                id = "F_" + a.Name,
                obj = a,
                text = a.Name,
                type = ContentsTreeNodeType.Container,
                children = a.FromSubmitEvents.Select(aa => new TreeNodeModel()
                {
                    id = aa.Id.ToString(),
                    obj = aa,
                    text = aa.Name,
                    type = ContentsTreeNodeType.Item
                }).ToList()
            });
            return Json(forms);
        }
    }
}
