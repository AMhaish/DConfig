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
    public class RUSecuritySettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public bool EmailRegConfirmation { get; set; }
        public bool SMSRegConfirmation { get; set; }
        public string MicrosoftClientId { get; set; }
        public string MicrosoftClientSecret { get; set; }
        public string TwitterClientId { get; set; }
        public string TwitterClientSecret { get; set; }
        public string FacebookClientId { get; set; }
        public string FacebookClientSecret { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var obj = new RUSecuritySettingsAction();
            obj.MicrosoftClientId= websiteSettingsAPI.Get(WebsiteSetting.Security_MicrosoftClientId);
            obj.MicrosoftClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_MicrosoftClientSecret);
            obj.TwitterClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_TwitterClientId);
            obj.TwitterClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_TwitterClientSecret);
            obj.FacebookClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_FacebookClientId);
            obj.FacebookClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_FacebookClientSecret);
            obj.GoogleClientId = websiteSettingsAPI.Get(WebsiteSetting.Security_GoogleClientId);
            obj.GoogleClientSecret = websiteSettingsAPI.Get(WebsiteSetting.Security_GoogleClientSecret);
            obj.EmailRegConfirmation = Convert.ToBoolean(websiteSettingsAPI.Get(WebsiteSetting.Security_EmailRegConfirmation));
            obj.SMSRegConfirmation = Convert.ToBoolean(websiteSettingsAPI.Get(WebsiteSetting.Security_SMSRegConfirmation));
            return Json(obj);
        }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            websiteSettingsAPI.Save(WebsiteSetting.Security_MicrosoftClientId, MicrosoftClientId);
            websiteSettingsAPI.Save(WebsiteSetting.Security_MicrosoftClientSecret, MicrosoftClientSecret);
            websiteSettingsAPI.Save(WebsiteSetting.Security_TwitterClientId, TwitterClientId);
            websiteSettingsAPI.Save(WebsiteSetting.Security_TwitterClientSecret, TwitterClientSecret);
            websiteSettingsAPI.Save(WebsiteSetting.Security_FacebookClientId, FacebookClientId);
            websiteSettingsAPI.Save(WebsiteSetting.Security_FacebookClientSecret, FacebookClientSecret);
            websiteSettingsAPI.Save(WebsiteSetting.Security_GoogleClientId, GoogleClientId);
            websiteSettingsAPI.Save(WebsiteSetting.Security_GoogleClientSecret, GoogleClientSecret);
            websiteSettingsAPI.Save(WebsiteSetting.Security_EmailRegConfirmation, EmailRegConfirmation.ToString());
            websiteSettingsAPI.Save(WebsiteSetting.Security_SMSRegConfirmation, SMSRegConfirmation.ToString());
            return Json(new { result = "true" });
        }

    }
}
