using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web.Hosting;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using System.IO;

namespace DConfigOS_Core.Layer2.SettingsServices
{
    public class RLogsSettingsAction : SABFramework.Core.SABAction
    {
        public List<string> Logs { get; set; }
        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            DirectoryInfo dir = new DirectoryInfo(HostingEnvironment.MapPath("~/logs/"));
            var obj = new RLogsSettingsAction();
            obj.Logs = dir.GetFiles().OrderByDescending(p => p.CreationTime).Select(m => Path.GetFileNameWithoutExtension(m.Name)).Take(10).ToList();

            return Json(obj);
        }
    }
}
