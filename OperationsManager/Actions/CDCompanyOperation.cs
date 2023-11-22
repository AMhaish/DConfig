using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using OperationsManager.OperationsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
using OperationsManager.Models;

namespace OperationsManager.Actions
{
    public class CDCompanyOperation : AppBaseAction
    {
        [Required]
        public int OperationId { get; set; }
        [Required]
        public int CompanyId { get; set; }
        
        public IOperationsAPI OperationsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                int? newId;
                var result = OperationsAPI.LinkOperationToCompany(OperationId,CompanyId,out newId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj= newId.Value });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "This link is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Name is required to add a new operation" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {

                var result = OperationsAPI.RemoveOperationFromCompany(OperationId, CompanyId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "This link hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Operation id and company id are required to remove the operation from the company" });
            }
        }
    }
}
