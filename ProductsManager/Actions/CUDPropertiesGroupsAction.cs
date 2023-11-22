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
    public class CUDPropertiesGroupsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int Priority { get; set; }
        public string DisplayAs { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new PropertiesGroup()
                {
                    Id = Id,
                    Name = Name,
                    Priority = Priority,
                    DisplayAs = DisplayAs
                };
                var result = propertiesAPI.CreatePropertiesGroup(g);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = g });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Group with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to add a group" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var g = new PropertiesGroup()
                {
                    Id = Id,
                    Name = Name,
                    Priority = Priority,
                    DisplayAs = DisplayAs
                };
                var result = propertiesAPI.UpdatePropertiesGroup(g);
                var gg = propertiesAPI.GetPropertiesGroup(g.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = gg });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "There is already a group with the same name" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Group hasn't been found to be updated or the name you chosen is already used in another group" });
                    default:
                        return Json(new { result = "false"});
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to update a group" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = propertiesAPI.DeletePropertiesGroup(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Group hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to delete a group" });
            }
        }
    }
}
