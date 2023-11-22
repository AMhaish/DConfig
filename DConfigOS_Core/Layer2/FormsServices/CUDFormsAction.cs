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
    public class CUDFormsAction : UserActionsBase
    {
        [Inject]
        public IFormsAPI formsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public int? ParentFormId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Type { get; set; }
        public string SubmitButtonText { get; set; }
        public string SubmitRedirectUrl { get; set; }
        public string NextButtonText { get; set; }
        public string BackButtonText { get; set; }
        public string AddItemButtonText { get; set; }
        public string RemoveItemButtonText { get; set; }
        public int? PrintTemplateId { get; set; }
        public bool? ReCapatchaEnabled { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var form = new Form()
                {
                    Id = Id,
                    Name = Name,
                    Type = Type,
                    ParentFormId = ParentFormId,
                    SubmitButtonText = SubmitButtonText,
                    SubmitRedirectUrl = SubmitRedirectUrl,
                    NextButtonText = NextButtonText,
                    BackButtonText = BackButtonText,
                    AddItemButtonText = AddItemButtonText,
                    RemoveItemButtonText = RemoveItemButtonText,
                    PrintTemplateId = PrintTemplateId,
                    CreatorId=UserId,
                    ReCapatchaEnabled = ReCapatchaEnabled,
                };
                var result = formsAPI.CreateForm(form);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = form });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Form with the same name is already exists" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to add a form" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var form = new Form()
                {
                    Id = Id,
                    Name = Name,
                    Type = Type,
                    ParentFormId = ParentFormId,
                    SubmitButtonText = SubmitButtonText,
                    SubmitRedirectUrl = SubmitRedirectUrl,
                    NextButtonText = NextButtonText,
                    BackButtonText = BackButtonText,
                    AddItemButtonText = AddItemButtonText,
                    RemoveItemButtonText = RemoveItemButtonText,
                    PrintTemplateId = PrintTemplateId,
                    ReCapatchaEnabled = ReCapatchaEnabled,
                };
                var result = formsAPI.UpdateForm(form);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var obj = formsAPI.GetForm(Id);
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Form hasn't been found to be updated" });
                    case ResultCodes.ObjectNotAllowedToBeUpdated:
                        return Json(new { result = "false", message = "Form is not allowed to be updated cause its linked with an app" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to update a form" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = formsAPI.DeleteForm(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Form hasn't been found to be deleted" });
                    case ResultCodes.ObjectNotAllowedToBeDeleted:
                        return Json(new { result = "false", message = "Form is not allowed to be deleted cause its linked with an app" });
                    case ResultCodes.ObjectLinkedToAnotherObject:
                        return Json(new { result = "false", message = "Already users have posted data to this form, please delete all these data before deleting this form" });

                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to delete a form" });
            }
            return null;
        }
    }
}
