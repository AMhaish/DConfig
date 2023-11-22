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
    public class CTemplateCloneAction : UserActionsBase
    {
        [Inject]
        public ITemplatesAPI templatesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public string Suffix { get; set; }
        
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                ViewTemplate template;
                var result = templatesAPI.CloneTemplate(Id, out template, Suffix);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = template });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Template hasn't been found to be cloned." });
                }
            }
            return Json(new { result = "false", message = "Id and Name are required" });
        }

    }
}
