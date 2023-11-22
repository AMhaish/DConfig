using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class AllAppsAction: UserActionsBase
    {
        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
             return Json(appAPIRepository.GetInstalledApps());
        }
    }
}
