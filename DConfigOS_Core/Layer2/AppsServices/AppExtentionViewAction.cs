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
    public class AppExtentionViewAction : SABFramework.Core.SABAction
    {
        public int id { get; set; }

        [Inject]
        public IAppsExtentionsAPI appsExtentionsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            StringBuilder result=new StringBuilder();
            AppExtention ext = appsExtentionsAPI.GetAppExtention(id);
            foreach(AppExtentionView ev in ext.AppExtViews)
            {
                result.Append(RenderViewToString(controller,ev.Path));
            }
            return Str(result.ToString());
        }
    }
}
