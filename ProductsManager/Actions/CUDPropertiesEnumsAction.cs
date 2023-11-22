using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class CUDPropertiesEnumsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesEnumsAPI propertiesEnumsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var f = new PropertyEnum()
                {
                    Id = Id,
                    Name = Name
                };
                var result = propertiesEnumsAPI.CreatePropertyEnum(f);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = f });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Predefined list with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var f = new PropertyEnum()
                {
                    Id = Id,
                    Name = Name
                };
                var result = propertiesEnumsAPI.UpdatePropertyEnum(f);
                var p = propertiesEnumsAPI.GetPropertiesEnum(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = p });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Predefined list hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "There is already predefined list with the same name" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = propertiesEnumsAPI.DeletePropertyEnum(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Predefined list hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
        }
    }
}
