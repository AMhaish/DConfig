using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetPropertiesGroupsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var groups = propertiesAPI.GetPropertiesGroups();
            return Json(groups);
        }

    }
}
