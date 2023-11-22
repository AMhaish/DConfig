using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using OperationsManager.OperationsManagerServices;

namespace OperationsManager.Actions
{
    public class RenderCurrentOperationsStateAction : SABFramework.Core.SABAction
    {
        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = OperationsInstancesAPI.GetActiveOperations(DateTime.Now.Month, DateTime.Now.Year, null, null, null, null);
            return View(result);
        }
    }
}
