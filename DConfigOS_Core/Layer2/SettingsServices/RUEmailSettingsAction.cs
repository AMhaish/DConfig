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
    public class RUEmailSettingsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IWebsiteSettingsAPI websiteSettingsAPI { get; set; }

        public int Port { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string From { get; set; }
        public bool SSL { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var model = new RUEmailSettingsAction();
            //var config = WebConfigurationManager.OpenWebConfiguration(controller.Request.ApplicationPath);
            //var settings = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
            //if (settings != null)
            //{
            //    model.Port = settings.Smtp.Network.Port;
            //    model.Host = settings.Smtp.Network.Host;
            //    model.UserName = settings.Smtp.Network.UserName;
            //    model.Password = settings.Smtp.Network.Password;
            //    model.From = settings.Smtp.From;
            //}
            model.Port = Convert.ToInt32(websiteSettingsAPI.Get(WebsiteSetting.SMTP_Port));
            model.Host = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Host);
            model.UserName = websiteSettingsAPI.Get(WebsiteSetting.SMTP_UserName);
            model.Password = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Password);
            model.From = websiteSettingsAPI.Get(WebsiteSetting.SMTP_From);
            model.SSL = Convert.ToBoolean(websiteSettingsAPI.Get(WebsiteSetting.SMTP_SSL));
            return Json(model);
        }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            try
            {
                SmtpClient sc = new SmtpClient(Host, Port);
                ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;

                sc.Credentials = new System.Net.NetworkCredential(UserName, Password);
                MailMessage m = new MailMessage(From, From);
                m.Body = "test";
                m.Subject = "test";
                sc.EnableSsl = SSL;
                sc.Send(m);

                //Configuration config = WebConfigurationManager.OpenWebConfiguration(controller.Request.ApplicationPath);
                //MailSettingsSectionGroup mailSection = config.GetSectionGroup("system.net/mailSettings") as MailSettingsSectionGroup;
                //mailSection.Smtp.From = From;
                //mailSection.Smtp.Network.Host = Host;
                //mailSection.Smtp.Network.UserName = UserName;
                //mailSection.Smtp.Network.Password = Password;
                //mailSection.Smtp.Network.Port = Port;
                //mailSection.Smtp.Network.EnableSsl = SSL;

                //config.Save(ConfigurationSaveMode.Modified);

                websiteSettingsAPI.Save(WebsiteSetting.SMTP_Host, Host);
                websiteSettingsAPI.Save(WebsiteSetting.SMTP_Port, Port.ToString());
                websiteSettingsAPI.Save(WebsiteSetting.SMTP_UserName, UserName);
                websiteSettingsAPI.Save(WebsiteSetting.SMTP_From, From);
                websiteSettingsAPI.Save(WebsiteSetting.SMTP_Password, Password);
                websiteSettingsAPI.Save(WebsiteSetting.SMTP_SSL, SSL.ToString());
                return Json(new { result = "true" });
            }
            catch(SmtpException smtpEx)
            {
                return Json(new { result = "false", message = smtpEx.Message });
            }                           
            catch (Exception ex)
            {
                return Json(new { result = "false", message = ex.Message });
            }
        }

    }
}
