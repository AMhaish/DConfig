using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompetitiveAnalysis.ProductsManagerServices;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetPropertiesTypesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var types = propertiesAPI.GetPropertiesTypes();
            return Json(types);
        }
    }
}
