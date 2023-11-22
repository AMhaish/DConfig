using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer1.CoreServices;
using DConfigOS_Core.Layer1.AppsServices;
using DConfigOS_Core.Layer1.Utilities;

namespace DConfigOS_Core.Layer2.CoreServices
{
    public class CheckUpdatesAction : BaseAction
    {
        //public string packageId { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var projectManager = GetCoreManager(controller);
            var installedPackage = GetInstalledPackage(projectManager, "DConfig");
            //var update = projectManager.GetUpdate(installedPackage);
            //var model = new InstallationState
            //{
            //    Installed = installedPackage,
            //    Update = update
            //};
            //var data = new
            //{
            //    result= "true",
            //    Version = (update != null) ? update.Version.ToString() : null,
            //    UpdateAvailable = update != null
            //};
            //return Json(data);
            string lastVersion;
            int lastVersion_num;
            int currentVersion_num;
            var result = AppsAPI.CheckAppNewUpdates("DConfig-Core", out lastVersion);
            currentVersion_num = int.Parse(installedPackage.Version.ToString().Replace(".", ""));
            lastVersion_num = int.Parse(lastVersion.Replace(".", ""));
            switch (result)
            {
                case ResultCodes.Succeed:
                    return Json(new { result = "true", obj = new { NewUpdates = (lastVersion_num > currentVersion_num ? "true" : "false"), NewVersion = lastVersion } });
                default:
                    return Json(new { result = "false", message = "Unknown error, please contact support team." });
            }
        }
    }
}
