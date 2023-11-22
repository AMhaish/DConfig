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
    public class DesktopWidgetsAction : SABFramework.Core.SABAction
    {
        public int? id { get; set; }
        public int? order { get; set; }

        [Inject]
        public IWidgetsAPIRepository widgetsAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            return Json(widgetsAPIRepository.GetDesktopWidgets());
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(System.Web.Mvc.Controller controller)
        {
            if (id.HasValue)
            {
                var result = widgetsAPIRepository.RemoveWidgetFromDesktop(id.Value);
                if (result==ResultCodes.Succeed)
                {
                    return Json(  new { result = "true" });
                }
                else
                {
                    return Json( new { result = "false", message = "No widget on desktop with the id:" + id.Value });
                }
            }
            else
            {
                return Json( new { result = "false", message = "Widget id is missing" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            if (id.HasValue)
            {
                if (!order.HasValue)
                {
                    order = int.MaxValue;
                }
                var result = widgetsAPIRepository.AddWidgetToDesktop(id.Value, order.Value);
                if (result==ResultCodes.Succeed)
                {
                   return Json(new { result = "true" });
                }
                else
                    return Json( new { result = "false", message = "No widget with the id:" + id.Value });
            }
            else
                return Json( new { result = "false", message = "Widget id is missing" });
        }
    }
}
