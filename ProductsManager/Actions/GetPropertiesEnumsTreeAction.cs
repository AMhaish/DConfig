using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.ProductsManagerServices;
using CompetitiveAnalysis.Models;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetPropertiesEnumsTreeAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesEnumsAPI propertiesEnumsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = propertiesEnumsAPI.GetPropertiesEnums().Select(a => new TreeNodeModel()
            {
                id = a.Id.ToString(),
                obj = a,
                text = a.Name
            });
            return Json(result);
        }
    }
}
