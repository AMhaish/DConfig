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
    public class CUDContentsAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Online { get; set; }
        public bool PlentyChildren { get; set; }
        public string UrlName { get; set; }
        public int? ViewTypeId { get; set; }
        public int? Priority { get; set; }
        public ContentTypes ContentType { get; set; }
        public int? StageId { get; set; }
        public DateTime? DueDate { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var content = new Content()
                {
                    Id = Id,
                    Name = Name,
                    Online = Online,
                    UrlName = UrlName?.Replace("?", "").Replace(":", "").Replace("+", "").Replace(" ", "").Replace(".", ""),
                    ViewTypeId = ViewTypeId,
                    ParentId = ParentId,
                    ContentType = ContentType,
                    PlentyChildren = PlentyChildren,
                    CreatorId = UserId,
                    StageId = StageId,
                    DueDate = DueDate
                };
                var result = contentsAPI.CreateContent(content);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var currentContent = contentsAPI.GetContent(content.Id);
                        if (currentContent.ParentId.HasValue)
                        {
                            var parentContent = contentsAPI.GetContent(currentContent.ParentId.Value);
                            if (parentContent.ViewType != null)
                            {
                                currentContent.PossibleViewTypes = parentContent.ViewType.ChildrenTypes.ToList();
                            }
                            else
                            {
                                currentContent.PossibleViewTypes = viewTypesAPI.GetRootViewTypes();
                            }
                        }
                        else
                        {
                            currentContent.PossibleViewTypes = viewTypesAPI.GetRootViewTypes();
                        }
                        if (currentContent.ViewType != null)
                        {
                            currentContent.PossibleChildViewTypes = currentContent.ViewType.ChildrenTypes.ToList();
                            currentContent.PossibleChildViewTemplates = currentContent.ViewType.TypeTemplates.ToList();
                            //currentContent.ViewType = currentContent.ViewType;
                        }
                        return Json(new { result = "true", obj = currentContent });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Content with the same name is already exists" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Parent hasn't been found to create this content" });
                }
            }
            return Json(new { result = "false", message = "Id and Name are required" });
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var content = new Content()
                {
                    Id = Id,
                    Name = Name,
                    Online = Online,
                    UrlName = UrlName?.Replace("?", "").Replace(":", "").Replace("+", "").Replace(" ", "").Replace(".", ""),
                    ParentId = ParentId,
                    PlentyChildren = PlentyChildren,
                    StageId = StageId,
                    DueDate = DueDate
                };
                if (Priority != null && Priority.HasValue)
                    content.Priority = Priority.Value;

                var result = contentsAPI.UpdateContent(content);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        if (content.ParentId.HasValue)
                        {
                            content.PossibleViewTypes = contentsAPI.GetContent(content.ParentId.Value).ViewType.ChildrenTypes.ToList();
                        }
                        else
                        {
                            content.PossibleViewTypes = viewTypesAPI.GetRootViewTypes();
                        }
                        var currentContent = contentsAPI.GetContent(content.Id);
                        if (currentContent.ViewType != null)
                        {
                            currentContent.PossibleChildViewTypes = currentContent.ViewType.ChildrenTypes.ToList();
                            //content.PossibleChildViewTemplates = currentContent.ViewType.TypeTemplates.ToList();
                        }
                        return Json(new { result = "true", obj = currentContent });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Content Name or Url Name have already been used in another content, please check and try again." });
                }
            }
            return Json(new { result = "false", message = "Id and Name are required" });
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = contentsAPI.DeleteContent(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content hasn't been found to be deleted" });
                }
            }
            return Json(new { result = "false", message = "Id and Name are required" });
        }
    }
}
