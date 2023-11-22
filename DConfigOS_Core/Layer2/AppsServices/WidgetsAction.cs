using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class WidgetsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWidgetsAPIRepository widgetsAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            return Json(widgetsAPIRepository.GetInstalledWidgets());

        }
    }
}
