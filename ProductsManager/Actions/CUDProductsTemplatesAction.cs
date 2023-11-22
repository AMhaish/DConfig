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
    public class CUDProductsTemplatesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new ProductsTemplate()
                {
                    Id = Id,
                    Name = Name
                };
                var result = productsAPI.CreateTemplate(g);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Template with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to add a template" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new ProductsTemplate()
                {
                    Id = Id,
                    Name = Name
                };
                var result = productsAPI.UpdateTemplate(g);
                var gg = productsAPI.GetTemplate(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = gg });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Template hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "There is already template with the same name" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to update a template" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = productsAPI.DeleteTemplate(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Template hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to delete a template" });
            }
        }
    }
}
