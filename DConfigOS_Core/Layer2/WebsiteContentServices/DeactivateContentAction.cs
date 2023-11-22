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
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class DeactivateContentAction : UserActionsBase
    {
        [Required]
        public int Id { get; set; }

        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = contentsAPI.DeactivateContent(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectAlreadyUpdated:
                        return Json(new { result = "false", message = "Content have been already deactivated" });
                    case ResultCodes.ObjectHasntFound:
                    default:
                        return Json(new { result = "false", message = "Content hasn't been found to be deactivated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required for deactivating" });
            }
        }

    }
}
