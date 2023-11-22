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
using System.Web.Configuration;
using System.Net.Configuration;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.SettingsServices
{
    public class RUSMSSettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public string Twilio_AccountSid { get; set; }
        public string Twilio_AuthToken { get; set; }
        public string Twilio_From { get; set; }
        public string ASPSMS_AccountSid { get; set; }
        public string ASPSMS_AuthToken { get; set; }
        public string ASPSMS_From { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var model = new RUSMSSettingsAction();
            model.Twilio_AccountSid = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_AccountSid);
            model.Twilio_AuthToken = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_AuthToken);
            model.Twilio_From = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_From);
            model.ASPSMS_AccountSid = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_AccountSid);
            model.ASPSMS_AuthToken = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_AuthToken);
            model.ASPSMS_From = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_From);
            return Json(model);
        }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            websiteSettingsAPI.Save(WebsiteSetting.SMS_Twilio_AccountSid, Twilio_AccountSid);
            websiteSettingsAPI.Save(WebsiteSetting.SMS_Twilio_AuthToken, Twilio_AuthToken);
            websiteSettingsAPI.Save(WebsiteSetting.SMS_Twilio_From, Twilio_From);
            websiteSettingsAPI.Save(WebsiteSetting.SMS_ASPSMS_AccountSid, ASPSMS_AccountSid);
            websiteSettingsAPI.Save(WebsiteSetting.SMS_ASPSMS_AuthToken, ASPSMS_AuthToken);
            websiteSettingsAPI.Save(WebsiteSetting.SMS_ASPSMS_From, ASPSMS_From);
            return Json(new { result = "true" });
        }

    }
}
