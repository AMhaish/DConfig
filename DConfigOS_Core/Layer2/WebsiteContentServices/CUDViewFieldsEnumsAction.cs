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
    public class CUDViewFieldsEnumsAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsEnumsAPI viewFieldsEnumsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var f = new ViewFieldsEnum()
                {
                    Id = Id,
                    Name = Name,
                    CreatorId=UserId
                };
                var result = viewFieldsEnumsAPI.CreateViewFieldsEnum(f);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = f });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Enum with the same name is already exists" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var f = new ViewFieldsEnum()
                {
                    Id = Id,
                    Name = Name
                };
                var result = viewFieldsEnumsAPI.UpdateFormsFieldsEnum(f);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = f });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Enum hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = viewFieldsEnumsAPI.DeleteFormsFieldsEnum(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Enum hasn't been found to be deleted" });
                    case ResultCodes.ObjectLinkedToAnotherObject:
                        return Json(new { result = "false", message = "These are still form fields that are linked to this list, please remove all linking then try again" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required" });
            }
            return null;
        }
    }
}
