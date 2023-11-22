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
    public class UTemplateLayoutAction : UserActionsBase
    {
        [Inject]
        public ITemplatesAPI templatesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public int? LayoutTemplateId { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = templatesAPI.UpdateTemplateLayout(Id, LayoutTemplateId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(  new { result = "true" });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json( new { result = "false", message = "Template hasn't been found to be updated" });
                }
            }
            return null;
        }
    }
}
