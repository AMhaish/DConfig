using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UDStageRolesAction : UserActionsBase
    {
        [Inject]
        public IStagesAPI stagesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var res = stagesAPI.AddStageRoles(Id, RoleId);
            if (res == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Stage hasn't been found to be updated" });
            }
        }


        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var res = stagesAPI.DeleteStageRoles(Id, RoleName);
            if (res == ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Value hasn't been found to be deleted" });
            }
        }

    }
}
