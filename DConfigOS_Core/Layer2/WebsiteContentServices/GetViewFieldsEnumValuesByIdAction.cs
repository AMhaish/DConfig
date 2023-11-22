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
    public class GetViewFieldsEnumValuesByIdAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsEnumsAPI viewFieldsEnumsAPI { get; set; }

        public string keyword { get; set; }
        public int limit { get; set; }
        public int skip { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public int Id { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = viewFieldsEnumsAPI.GetViewFieldsEnumValues(Id, limit, skip, keyword, (UserBasedApps ? UserId : null), sortField, sortOrder);
            return Json(result);
        }
    }
}
