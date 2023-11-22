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
    public class GetRootViewTypesAction : UserActionsBase
    {
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        public int? templateContextId { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (templateContextId.HasValue)
            {
                var result = viewTypesAPI.GetRootViewTypesByContextId(templateContextId.Value);
                return Json(result);
            }
            else
            {
                var result = viewTypesAPI.GetRootViewTypes((UserBasedApps ? UserId : null));
                return Json(result);
            }
        }

    }
}
