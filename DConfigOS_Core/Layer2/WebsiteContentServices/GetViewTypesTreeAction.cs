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
    public class GetViewTypesTreeAction : UserActionsBase
    {
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = viewTypesAPI.GetViewTypes( (UserBasedApps ? UserId : null)).Select(a => new TreeNodeModel()
            {
                id = a.Id.ToString(),
                obj = a,
                text = a.Name
            });
            return Json(result);
        }

    }
}
