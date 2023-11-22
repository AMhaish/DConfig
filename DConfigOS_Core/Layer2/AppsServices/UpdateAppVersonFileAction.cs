using System;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class UpdateAppVersonFileAction : SABFramework.Core.SABAction
    {
        public string AppName { get; set; }
        public string Version { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            string downloadedFilePath = null;
            var result = appAPIRepository.DownloadAppVersionFile(AppName,Version,Guid.Parse(SABFramework.Core.SABCoreEngine.Instance.Settings["DeviceKey"]),UserName,Password, out downloadedFilePath);
            switch (result)
            {
                case ResultCodes.Succeed:
                    appAPIRepository.InstallApp(downloadedFilePath);
                    return Json(new { result = "true" });
                default:
                    return Json(new { result = "false", message = "Unknown error, please contact support team." });
            }
        }
    }
}
