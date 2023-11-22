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
    public class GetViewFieldsEnumsAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsEnumsAPI viewFieldsEnumsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = viewFieldsEnumsAPI.GetViewFieldsEnums( (UserBasedApps ? UserId : null));
            return Json(result);
        }
    }
}
