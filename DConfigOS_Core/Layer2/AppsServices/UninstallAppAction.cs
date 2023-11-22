using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using System.IO;
using System.Web.Optimization;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class UninstallAppAction : SABFramework.Core.SABAction
    {
        public string Name { get; set; }

        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = appAPIRepository.UninstallApp(Name);
            if (result)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Uninstalling failed, please check the logs to found out the problem" });
            }
        }
    }
}
