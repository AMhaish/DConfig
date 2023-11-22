using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class CheckAppUpdates : SABFramework.Core.SABAction
    {
        [Inject]
        private IAppAPIRepository appAPIRepository { get; set; }

        public string AppName { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            string lastVersion;
            int lastVersion_num;
            int currentVersion_num;
            var app = appAPIRepository.GetAppInfo(AppName);
            if (app == null)
            {
                return Json(new { result = "false", message = "App couldn't be found, please contact support team." });
            }
            var result = appAPIRepository.CheckAppNewUpdates(AppName, out lastVersion);
            currentVersion_num = int.Parse(app.Version.Replace(".", ""));
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
