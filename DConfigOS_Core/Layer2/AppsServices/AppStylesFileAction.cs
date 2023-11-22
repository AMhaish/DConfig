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
    public class AppStylesFileAction : SABFramework.Core.SABAction
    {
        public string id { get; set; }

        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
#if DEBUG
            //Bundle sb = new Bundle("~/Content/" + id);
#else
            //StyleBundle sb = new StyleBundle("~/Content/" + id);
#endif
            Bundle sb = new Bundle("~/Content/" + id);
            if (!BundleTable.Bundles.Contains(sb))
            {
                //var context = new BundleContext(controller.HttpContext, BundleTable.Bundles, string.Empty);
                var styles = appAPIRepository.GetAppStyles(id);
                string[] scriptspath= styles.Select(m => "~" + m.Path).ToArray();
                sb.Include(scriptspath);
                BundleTable.Bundles.Add(sb);
            }
            return Json(  new { data = BundleTable.Bundles.ResolveBundleUrl("~/Content/" + id) });
        }
    }
}
