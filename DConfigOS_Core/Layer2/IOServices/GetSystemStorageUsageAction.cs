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
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Repositories.Utilities;
using System.IO;

namespace DConfigOS_Core.Layer2.IOServices
{
    public class GetSystemStorageUsageAction : SABFramework.Core.SABAction
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(Controller controller)
        {
            DirectoryInfo d = new DirectoryInfo(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath);
            return Json(d.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length));
        }

    }
}
