using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationsManager.OperationsManagerServices;

namespace OperationsManager.Actions
{
    public class GetOperationsCategoriesAction : SABFramework.Core.SABAction
    {
        public IOperationsAPI OperationsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = OperationsAPI.GetOperationsCategories();
            return Json(result);
        }
    }
}
