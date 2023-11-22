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
    public class CContentCloneAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public string Suffix { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                Content content;
                var result = contentsAPI.CloneContent(Id, out content , Suffix);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        if (content.ParentId.HasValue)
                        {
                            var parentContent = contentsAPI.GetContent(content.ParentId.Value);
                            if (parentContent.ViewType != null)
                            {
                                content.PossibleViewTypes = parentContent.ViewType.ChildrenTypes.ToList();
                            }
                            else
                            {
                                content.PossibleViewTypes = viewTypesAPI.GetRootViewTypes();
                            }
                        }
                        else
                        {
                            content.PossibleViewTypes = viewTypesAPI.GetRootViewTypes();
                        }
                        var currentContent = contentsAPI.GetContent(content.Id);
                        if (currentContent.ViewType != null)
                        {
                            content.PossibleChildViewTypes = currentContent.ViewType.ChildrenTypes.ToList();
                            content.ViewType = currentContent.ViewType;
                        }
                        return Json(new { result = "true", obj = content });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content hasn't been found to be cloned." });
                }
            }
            return Json(new { result = "false", message = "Id and Name are required" });
        }

    }
}
