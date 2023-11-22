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
    public class CUDOperationsCategoryAction : SABFramework.Core.SABAction
    {
        public int? Id { get; set; }
        public string Name { get; set; }

        public IOperationsAPI OperationsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(Name))
            {
                var g = new OperationsCategory()
                {
                    Name = Name
                };
                var result = OperationsAPI.CreateOperationsCategory(g);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "The category is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Name is required to add a new operations category" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (Id.HasValue && !String.IsNullOrEmpty(Name))
            {
                var g = new OperationsCategory()
                {
                    Id=Id.Value,
                    Name = Name
                };
                var result = OperationsAPI.UpdateOperationsCategory(g);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "The Category hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and name are required to update an operations category" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (Id.HasValue)
            {
                var result = OperationsAPI.DeleteOperationsCategory(Id.Value);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "The Category hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required to delete an operation category" });
            }
        }
    }
}
