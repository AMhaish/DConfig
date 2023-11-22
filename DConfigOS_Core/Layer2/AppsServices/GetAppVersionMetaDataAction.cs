using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class GetAppVersionMetaDataAction : SABFramework.Core.SABAction
    {
        public string AppName { get; set; }
        public string Version { get; set; }

        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            FileMetaData metaData;
            var result = appAPIRepository.GetAppVersionMetaData(AppName, Version, out metaData);
            switch (result)
            {
                case ResultCodes.Succeed:
                    return Json(new { result = "true", obj = metaData });
                default:
                    return Json(new { result = "false", message = "Unknown error, please contact support team." });
            }
        }
    }
}
