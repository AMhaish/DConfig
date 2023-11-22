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
    public class UDCompanyOperationAssignees : AppBaseAction
    {
        [Required]
        public int Id { get; set; }
        public List<string> UserIds { get; set; }
        public string UserId { get; set; }
        
        public IOperationsAPI OperationsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {

                var result = OperationsAPI.AssignUsersToCompanyOperation(Id, UserIds);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Company operation hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Company operation id is required to assign users to the company operation" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {

                var result = OperationsAPI.UnassignUserFromCompanyOperation(Id, UserId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Company operation hasn't been found to be updated" });
                    case ResultCodes.ObjectResourceHasntFound:
                        return Json(new { result = "false", message = "The user hasn't appeared to be linked with this operation, plesae try to refreh your page" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Company operation id is required to assign users to the company operation" });
            }
        }
    }
}
