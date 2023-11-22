using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using System.IO;
using System.Web.Optimization;
using DConfigOS_Core.Models;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class RenderScriptsTags : SABFramework.Core.SABAction
    {
        public List<int> ids { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            Bundle sb = null;
            //if (DConfigOS_Core.DConfigOSInitializer.OSinDebugMode)
            //{
            //sb = new Bundle("~/Content/" + ids);
            //}
            //else
            //{
            sb = new StyleBundle("~/Content/" + ids);
            //}

            if (!BundleTable.Bundles.Contains(sb))
            {
                var context = new BundleContext(controller.HttpContext, BundleTable.Bundles, string.Empty);
                List<StyleSheet> styles = new List<StyleSheet>();
                string[] scriptspath = styles.Select(m => "~" + m.Path).ToArray();
                sb.Include(scriptspath);
                BundleTable.Bundles.Add(sb);
            }
            return Json(new { data = BundleTable.Bundles.ResolveBundleUrl("~/Content/" + ids) });
        }
    }
}
