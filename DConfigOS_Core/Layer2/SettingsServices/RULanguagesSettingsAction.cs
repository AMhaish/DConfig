using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.SettingsServices
{
    public class RULanguagesSettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public string PublicDefaultLanguage { get; set; }
        public string PortalLanguage { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var obj = new RULanguagesSettingsAction();
            string lang = websiteSettingsAPI.Get(WebsiteSetting.Language_PublicDefaultLanguage);
            string portallang = websiteSettingsAPI.Get(WebsiteSetting.Language_PortalLanguage);
            if (lang!=null)
            {
                obj.PublicDefaultLanguage = lang;
            }
            else
            {
                obj.PublicDefaultLanguage = "EN";
            }
            if (lang != portallang)
            {
                obj.PortalLanguage = portallang;
            }
            else
            {
                obj.PortalLanguage = "EN";
            }
            return Json(obj);
        }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            websiteSettingsAPI.Save(WebsiteSetting.Language_PublicDefaultLanguage, PublicDefaultLanguage);
            websiteSettingsAPI.Save(WebsiteSetting.Language_PortalLanguage, PortalLanguage);
            return Json(new { result = "true" });
        }

    }
}
