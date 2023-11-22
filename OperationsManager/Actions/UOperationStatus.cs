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
    public class UOperationStatus : AppBaseAction
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }

        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {

                var result = OperationsInstancesAPI.ChangeOperationInstanceStatus(Id,User.Id, Status, Description);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Operation couldn't be found to update its status" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "id and status are required to change operation status" });
            }
        }
    }
}
