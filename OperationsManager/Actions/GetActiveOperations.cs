using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OperationsManager.OperationsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
using OperationsManager.Models;

namespace OperationsManager.Actions
{
    public class GetActiveOperations : SABFramework.Core.SABAction
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? CompanyId { get; set; }
        public string Status { get; set; }
        public int? CategoryId { get; set; }

        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = OperationsInstancesAPI.GetActiveOperations(Month, Year, null, CompanyId, Status, CategoryId);
            return Json(result);
        }
    }
}
