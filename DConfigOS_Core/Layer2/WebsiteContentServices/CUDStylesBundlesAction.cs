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
    public class CUDStylesBundlesAction : UserActionsBase
    {
        [Inject]
        public IStylesBundlesAPI stylesBundlesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var stylesBundle = new StylesBundle()
                {
                    Id = Id,
                    Name = Name,
                    CreatorId = UserId
                };
                var result = stylesBundlesAPI.CreateStylesBundle(stylesBundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = stylesBundle });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Style bundle with the same name is already exists" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var stylesBundle = new StylesBundle()
                {
                    Id = Id,
                    Name = Name,
                    CreatorId = UserId
                };
                var result = stylesBundlesAPI.UpdateStylesBundle(stylesBundle);
                var bundle = stylesBundlesAPI.GetStylesBundle(Id);
                Providers.ResourcesProviders.BundlesProvider.Instance.ReInitializeStyleBundle(bundle);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = stylesBundle });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Style bundle hasn't been found to be updated" });
                }
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var bundle = stylesBundlesAPI.GetStylesBundle(Id);
                Providers.ResourcesProviders.BundlesProvider.Instance.RemoveStyleBundle(bundle);
                var result = stylesBundlesAPI.DeleteStylesBundle(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Style bundle hasn't been found to be deleted" });
                }
            }
            return null;
        }
    }
}
