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
    public class CUDContentInstancesAction : UserActionsBase
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ContentId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Online { get; set; }
        public int? ViewTemplateId { get; set; }
        public string Language { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string Title { get; set; }
        public int Version { get; set; }
        public string RedirectUrl { get; set; }
        public string DownloadPath { get; set; }
        public string DownloadName { get; set; }
        public int? StageId { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var contentInstance = new ContentInstance()
                {
                    Id = Id,
                    Name = Name,
                    Online = Online,
                    ViewTemplateId = ViewTemplateId,
                    Language = Language,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords,
                    Title = Title,
                    Version = Version,
                    ContentId = ContentId,
                    DownloadName = DownloadName,
                    DownloadPath = DownloadPath,
                    RedirectUrl = RedirectUrl,                 
                    CreatorId = UserId,
                    StageId = StageId
                };
                var result = contentsAPI.CreateContentInstance(contentInstance);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var viewType = contentsAPI.GetContent(contentInstance.ContentId).ViewType;
                        //if (viewType != null)
                        //contentInstance.PossibleViewTemplates = viewType.TypeTemplates.ToList();
                        return Json(new { result = "true", obj = contentInstance });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Content Instance with the same name is already exists" });
                }
                return Json(new { result = "false", message = "Content Instance is missing data" });
            }
            else
            {
                return Json(new { result = "false", message = "Content Instance is missing data" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var contentInstance = new ContentInstance()
                {
                    Id = Id,
                    Name = Name,
                    Online = Online,
                    ViewTemplateId = ViewTemplateId,
                    Language = Language,
                    MetaDescription = MetaDescription,
                    MetaKeywords = MetaKeywords,
                    Title = Title,
                    Version = Version,
                    ContentId = ContentId,
                    DownloadName = DownloadName,
                    DownloadPath = DownloadPath,
                    RedirectUrl = RedirectUrl,
                    StageId = StageId
                };
                var result = contentsAPI.UpdateContentInstance(contentInstance,null);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var viewType = contentsAPI.GetContent(contentInstance.ContentId).ViewType;
                        //if (viewType != null)
                        //contentInstance.PossibleViewTemplates = viewType.TypeTemplates.ToList();
                        return Json(new { result = "true", obj = contentInstance });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content Instance hasn't been found to be updated" });
                }
                return Json(new { result = "false", message = "Content Instance is missing data" });
            }

            else
            {
                return Json(new { result = "false", message = "Content Instance is missing data" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = contentsAPI.DeleteContentInstance(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content Instance hasn't been found to be deleted" });
                }
            }
            return null;
        }
    }
}
