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
using System.IO;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class CUDViewTypesAction : UserActionsBase
    {
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var type = new DConfigOS_Core.Models.ViewType()
                {
                    Id = Id,
                    Name = Name,
                    CreatorId=UserId
                };
                var result = viewTypesAPI.CreateViewType(type);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = type });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Template with the same name is already exists" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var type = new DConfigOS_Core.Models.ViewType()
                {
                    Id = Id,
                    Name = Name
                };
                var result = viewTypesAPI.UpdateViewType(type);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = type });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "View type hasn't been found to be updated" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = viewTypesAPI.DeleteViewType(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "View type hasn't been found to be deleted" });
                    case ResultCodes.ObjectLinkedToAnotherObject:
                        return Json(new {result="false",message="View type can't been deleted cause one or more templates or contents is using it."});
                }
            }
            return null;
        }
    }
}
