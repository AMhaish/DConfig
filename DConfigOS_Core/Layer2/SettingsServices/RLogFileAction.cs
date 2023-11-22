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
    public class RLogFileAction : SABFramework.Core.SABAction
    {
        public string logFile { get; set; }
        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            if (String.IsNullOrEmpty(logFile))
            {
                logFile = DateTime.Now.ToString("dd-MM-yyyy");
            }
            string filePath = HostingEnvironment.MapPath("~/logs/" + logFile + ".csv");
            string fileData = System.IO.File.ReadAllText(filePath);
            fileData = fileData.Replace("\r\n", "\n");
            return Json(fileData);
        }
    }
}
