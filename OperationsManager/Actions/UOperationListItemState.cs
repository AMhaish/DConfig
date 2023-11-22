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
    public class UOperationListItemState : AppBaseAction
    {
        public int Id { get; set; }
        public bool Checked { get; set; }
        public IOperationsInstancesAPI OperationsInstancesAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {

                var result = OperationsInstancesAPI.SetAnOperationListItemCheckState(Id, Checked);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    default:
                        return Json(new { result = "false", message="List item hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required to update check list item state" });
            }
        }
    }
}
