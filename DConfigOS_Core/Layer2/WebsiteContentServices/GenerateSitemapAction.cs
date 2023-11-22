using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using DConfigOS_Core.Providers.HttpContextProviders;
using DConfigOS_Core.Providers.ResourcesProviders;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GenerateSitemapAction : SABFramework.Core.SABAction
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            SitemapProvider.Instance.InitializeSitemap(null, null, false);
            return Json(new { result = true });
        }
    }
}
