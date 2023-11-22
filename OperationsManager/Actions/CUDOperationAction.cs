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
    public class CUDOperationAction : AppBaseAction
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string Cycle { get; set; }
        public int RaiseDay { get; set; }
        public int DueOnDay { get; set; }
        public string StartTimeValue { get; set; }
        public string EndTimeValue { get; set; }
        public DateTime? StartingDate { get; set; }
        public string CreatingUserId { get; set; }
        public string Priority { get; set; }
        public virtual List<OperationCheckListItem> OperationCheckListItems { get; set; }

        public IOperationsAPI OperationsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new Operation()
                {
                    Name = Name,
                    Description=Description,
                    CategoryId=CategoryId,
                    Cycle=Cycle,
                    RaiseDay=RaiseDay,
                    DueOnDay=DueOnDay,
                    StartingDate=StartingDate,
                    CreatingUserId=User.Id,
                    Priority=Priority,
                    StartTimeValue = StartTimeValue,
                    EndTimeValue= EndTimeValue
                };
                var result = OperationsAPI.CreateOperation(g, OperationCheckListItems);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "The operation with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Name is required to add a new operation" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new Operation()
                {
                    Id=Id,
                    Name = Name,
                    Description = Description,
                    CategoryId = CategoryId,
                    Cycle = Cycle,
                    RaiseDay = RaiseDay,
                    DueOnDay = DueOnDay,
                    StartingDate = StartingDate,
                    CreatingUserId = User.Id,
                    Priority = Priority,
                    StartTimeValue = StartTimeValue,
                    EndTimeValue = EndTimeValue
                };
                var result = OperationsAPI.UpdateOperation(g,OperationCheckListItems);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "The operation hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and name are required to update an operation" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = OperationsAPI.DeleteOperation(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "The operation hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and name are required to delete an operation category" });
            }
        }
    }
}
