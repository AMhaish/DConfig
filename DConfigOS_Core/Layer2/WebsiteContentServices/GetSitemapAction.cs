using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using DConfigOS_Core.Providers.HttpContextProviders;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetSitemapAction : SABFramework.Core.SABAction
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var currentDomain = controller.HttpContext.Request.Url.Host;
            if (currentDomain == "localhost")
            {
                currentDomain = SABFramework.Core.SABCoreEngine.Instance.Settings["Domain"];
            }
            if (DConfigRequestContext.Domains.ContainsKey(currentDomain))
            {
                DConfigRequestContext.Current.DomainId = DConfigRequestContext.Domains[currentDomain];
                DConfigRequestContext.Current.ContextId = DConfigRequestContext.Contexts[currentDomain];
            }
            string sitemappath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\Sitemaps\\" + DConfigRequestContext.Current.ContextId.ToString();
            if (System.IO.File.Exists(sitemappath))
            {
                return XMLDocument(new System.IO.FileStream(sitemappath, System.IO.FileMode.Open));
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}
