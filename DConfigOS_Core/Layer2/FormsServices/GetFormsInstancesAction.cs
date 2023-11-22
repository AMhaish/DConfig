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

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class GetFormsInstancesAction : UserActionsBase
    {
        [Inject]
        public IFormsInstancesAPI formsInstancesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var formInstances = await formsInstancesAPI.GetFormInstances(Id);
                return Json(formInstances);
            }
            else
            {
                return Json(new { result = "false", message = "Form id is required" });
            }
        }

    }
}
