using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.ProductsManagerServices;
using CompetitiveAnalysis.Models;

namespace CompetitiveAnalysis.Actions
{
    public class GetAdvancedPrivilegesAction : SABFramework.Core.SABAction
    {
        public IAdvancedPrivilegesAPI AdvancedPrivilegesAPI { get; set;
        }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = AdvancedPrivilegesAPI.GetPrivileges();
            return Json(result);
        }
    }
}
