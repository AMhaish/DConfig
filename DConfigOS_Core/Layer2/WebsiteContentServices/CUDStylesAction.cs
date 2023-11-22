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
    public class CUDStylesAction : UserActionsBase
    {
        [Inject]
        public IStylesAPI stylesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Path { get; set; }
        public int? BundleId { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                string directory = "\\Content\\Public\\" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId;
                if (!Directory.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath +  directory))
                {
                    Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + directory);
                }
                Path = directory + "\\" + Name + ".css";
                var style = new StyleSheet()
                {
                    Id = Id,
                    Name = Name,
                    Path = Path,
                    BundleId=BundleId
                };
                var result = stylesAPI.CreateStyle(style);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = style });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Style with the same name is already exists" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                string oldPath = Path.Replace('/', '\\').TrimStart('~');
                Path = "\\Content\\Public\\" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "\\" + Name + ".css";
                var style = new StyleSheet()
                {
                    Id = Id,
                    Name = Name,
                    Path = Path
                };
                var result = stylesAPI.UpdateStyle(style);
                var bundle = stylesAPI.GetStyle(Id).Bundle;
                Providers.ResourcesProviders.BundlesProvider.Instance.ReInitializeStyleBundle(bundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        string oldFilePath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + oldPath;
                        if (!String.IsNullOrEmpty(oldPath) && File.Exists(oldFilePath))
                        {
                            File.Move(oldFilePath, SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path);
                        }
                        return Json(new { result = "true", obj = style });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Style hasn't been found to be updated" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && !String.IsNullOrEmpty(Path))
            {
                var bundle = stylesAPI.GetStyle(Id).Bundle;
                var result = stylesAPI.DeleteStyle(Id);
                Providers.ResourcesProviders.BundlesProvider.Instance.ReInitializeStyleBundle(bundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        Path = Path.Replace('/', '\\').TrimStart('~');
                        if (File.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path))
                        {
                            File.Delete(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + Path);
                        }
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Style hasn't been found to be deleted" });
                }
            }
            return null;
        }
    }
}
