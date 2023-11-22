using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationsManager.OperationsManagerServices;

namespace OperationsManager.Actions
{
    public class GenerateOperationsInstances : SABFramework.Core.SABAction
    {
        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            OperationsInstancesAPI.CreateOperationsInstances();
            return Json(new { result = "true" });
        }
    }
}
