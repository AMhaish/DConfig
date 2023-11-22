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
    public class GetUserActiveOperations : AppBaseAction
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? CompanyId { get; set; }

        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = OperationsInstancesAPI.GetActiveOperations((Month.HasValue?Month.Value :DateTime.Now.Month), (Year.HasValue ? Year.Value : DateTime.Now.Year), User.Id, CompanyId).Where(m => m.Status!="F").ToList();
            return Json(result);
        }
    }
}
