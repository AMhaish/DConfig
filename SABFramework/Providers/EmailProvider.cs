using Microsoft.AspNet.Identity;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace SABFramework.Providers
{
    public interface IEmailProvider : IIdentityMessageService
    {
        Task SendAsync(IdentityMessage message, int companyContextId);
    }

    public class EmailProvider : IEmailProvider
    {
        private IWebsiteSettingsAPI websiteSettingsAPI;

        public EmailProvider(IWebsiteSettingsAPI websiteSettingsAPI){
            this.websiteSettingsAPI = websiteSettingsAPI;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            var credential = new NetworkCredential("apikey", SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridSMTPKey"]);
            SmtpClient client = new SmtpClient(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridSMTPHost"], int.Parse(SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridSMTPPort"]));
            client.Credentials = credential;
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            await SendAsync(message, client, SABFramework.Core.SABCoreEngine.Instance.Settings["SendGridFrom"]);
        }

        public async Task SendAsync(IdentityMessage message, int companyContextId)
        {
            var smtp_host = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Host, companyContextId);
            var smtp_port = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Port, companyContextId);
            var smtp_username = websiteSettingsAPI.Get(WebsiteSetting.SMTP_UserName, companyContextId);
            var smtp_password = websiteSettingsAPI.Get(WebsiteSetting.SMTP_Password, companyContextId);
            var smtp_from = websiteSettingsAPI.Get(WebsiteSetting.SMTP_From, companyContextId);
            SmtpClient client = new SmtpClient(smtp_host, Convert.ToInt32(smtp_port));
            client.Credentials = new NetworkCredential(smtp_username, smtp_password);
            ServicePointManager.ServerCertificateValidationCallback += (o, c, ch, er) => true;
            await SendAsync(message, client, smtp_from);
        }

        public async Task SendAsync(IdentityMessage message, SmtpClient client, string from)
        {
            MailMessage m = new MailMessage();
            m.IsBodyHtml = true;
            m.From = new MailAddress(from);
            m.To.Add(message.Destination);
            m.Subject = message.Subject;
            if (message is SABIdentityMessage)
            {
                if(!String.IsNullOrEmpty((message as SABIdentityMessage).From))
                {
                    m.From = new MailAddress((message as SABIdentityMessage).From);
                }
                m.Body = SABFramework.Core.SABAction.RenderViewToString(((SABIdentityMessage)message).View, ((SABIdentityMessage)message).Model);
            }
            else
            {
                m.Body = message.Body;
            }
            await client.SendMailAsync(m);
        }

        public class SABIdentityMessage : IdentityMessage
        {
            public SABIdentityMessage()
            {
                BCCs = new MailAddressCollection();
                CCs = new MailAddressCollection();
            }
            public string From { get; set; }
            public string View { get; set; }
            public object Model { get; set; }
            public MailAddressCollection BCCs { get; set; }
            public MailAddressCollection CCs { get; set; }
        }
    }

}
