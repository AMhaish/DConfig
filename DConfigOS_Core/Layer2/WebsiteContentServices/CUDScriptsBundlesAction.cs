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
    public class CUDScriptsBundlesAction : UserActionsBase
    {
        [Inject]
        public IScriptsBundlesAPI scriptsBundlesAPI { get; set; } 

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var scriptsBundle = new ScriptsBundle()
                {
                    Id = Id,
                    Name = Name,
                    CreatorId = UserId
                };
                var result = scriptsBundlesAPI.CreateScriptsBundles(scriptsBundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = scriptsBundle });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Script bundle with the same name is already exists" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var scriptsBundle = new ScriptsBundle()
                {
                    Id = Id,
                    Name = Name
                };
                var result = scriptsBundlesAPI.UpdateScriptsBundle(scriptsBundle);
                var bundle = scriptsBundlesAPI.GetScriptBundle(Id);
                Providers.ResourcesProviders.BundlesProvider.Instance.ReInitializeScriptBundle(bundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = scriptsBundle });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Script bundle hasn't been found to be updated" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var bundle = scriptsBundlesAPI.GetScriptBundle(Id);
                Providers.ResourcesProviders.BundlesProvider.Instance.RemoveScriptBundle(bundle);
                var result = scriptsBundlesAPI.DeleteScriptsBundle(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Script bundle hasn't been found to be deleted" });
                }
            }
            return null;
        }
    }
}
