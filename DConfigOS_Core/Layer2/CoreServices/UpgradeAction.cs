using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using DConfigOS_Core.Layer1.CoreServices;
using DConfigOS_Core.Layer1.Utilities;
using DConfigOS_Core.Layer1.AppsServices;


namespace DConfigOS_Core.Layer2.CoreServices
{
    public class UpgradeAction : BaseAction
    {
        public string Version { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var coreManager = GetCoreManager(controller);
            var installedPackage = GetInstalledPackage(coreManager, "DConfig");
            string downloadedFilePath = null;
            var result = AppsAPI.DownloadAppVersionFile("DConfig", Version, Guid.Parse(SABFramework.Core.SABCoreEngine.Instance.Settings["DeviceKey"]), UserName, Password, out downloadedFilePath);
            switch (result)
            {
                case ResultCodes.Succeed:
                    var update = coreManager.GetUpdate(installedPackage);
                    if (update != null)
                    {
                        IEnumerable<string> updateResult=coreManager.UpdatePackage(update);
                        StringBuilder r = new StringBuilder();
                        foreach(string s in updateResult)
                        {
                            r.AppendLine(s);
                        }
                        return Json(new { result = "true", message= r.ToString() });
                    }
                    else { return Json(new { result = "false", message = "No update found." }); }
                default:
                    return Json(new { result = "false", message = "Unknown error, please contact support team." });
            }
        }
    }
}
