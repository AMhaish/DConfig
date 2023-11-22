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
    public class CUDTemplatesAction : UserActionsBase
    {
        [Inject]
        public ITemplatesAPI templatesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public int? LayoutTemplateId { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public int? ViewTypeId { get; set; }
        public bool IsContainer { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                string directory = "\\Views\\PublicViews\\Templates\\" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId;
                if(!Directory.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath +  directory))
                {
                    Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + directory);
                }
                Path = directory + "\\" + Name + ".cshtml";
                var template = new ViewTemplate()
                {
                    Id = Id,
                    Name = Name,
                    IsActive = IsActive,
                    Path = Path,
                    ViewTypeId = ViewTypeId,
                    LayoutTemplateId = LayoutTemplateId,
                    IsContainer = IsContainer,
                    CreatorId = UserId
                };
                var result = templatesAPI.CreateTemplate(template);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = template });
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
                string oldPath = Path.Replace('/', '\\').TrimStart('~');
                Path = "\\Views\\PublicViews\\Templates\\" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "\\" + Name + ".cshtml";
                var template = new ViewTemplate()
                {
                    Id = Id,
                    Name = Name,
                    IsActive = IsActive,
                    Path = Path,
                    ViewTypeId = ViewTypeId,
                    IsContainer=IsContainer,
                    LayoutTemplateId = LayoutTemplateId
                };
                var result = templatesAPI.UpdateTemplate(template);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        string oldFilePath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + oldPath;
                        if (!String.IsNullOrEmpty(oldPath) && File.Exists(oldFilePath))
                        {
                            File.Move(oldFilePath, SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path);
                        }
                        return Json(new { result = "true", obj = template });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Template hasn't been found to be updated" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && !String.IsNullOrEmpty(Path))
            {
                var result = templatesAPI.DeleteTemplate(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        if (File.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path.Replace('/', '\\').TrimStart('~')))
                        {
                            File.Delete(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path.Replace('/', '\\').TrimStart('~'));
                        }
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Template hasn't been found to be deleted" });
                    case ResultCodes.ObjectLinkedToAnotherObject:
                        return Json(new { result = "false", message = "Template can't been deleted cause one or more contents is using it." });
                }
            }
            return null;
        }
    }
}
