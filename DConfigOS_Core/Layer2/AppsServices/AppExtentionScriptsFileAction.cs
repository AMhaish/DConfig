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
    public class AppExtentionScriptsFileAction : SABFramework.Core.SABAction
    {
        public int id { get; set; }

        [Inject]
        public IAppsExtentionsAPI appsExtentionsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            //#if DEBUG
            Bundle sb = new Bundle("~/bundles/Ext" + id);
            //#else
            //ScriptBundle sb = new ScriptBundle("~/bundles/Ext" + id);
            //#endif

            if (!BundleTable.Bundles.Contains(sb))
            {
                var context = new BundleContext(controller.HttpContext, BundleTable.Bundles, string.Empty);
                var scripts = appsExtentionsAPI.GetAppExtentionClientLogics(id);
                string[] scriptspath = scripts.Select(m => "~" + m.Path).ToArray();
                sb.Include(scriptspath);

                BundleTable.Bundles.Add(sb);
            }
            return Json(new { data = BundleTable.Bundles.ResolveBundleUrl("~/bundles/Ext" + id) });
        }
    }
}
