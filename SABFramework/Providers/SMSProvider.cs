using Microsoft.AspNet.Identity;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

namespace SABFramework.Providers
{
    public interface ISMSProvider : IIdentityMessageService {
        Task SendAsync(IdentityMessage message, int companyContextId);
    }

    public class SMSProvider : ISMSProvider
    {
        private IWebsiteSettingsAPI websiteSettingsAPI;

        public SMSProvider(IWebsiteSettingsAPI websiteSettingsAPI)
        {
            this.websiteSettingsAPI = websiteSettingsAPI;
        }

        public Task SendAsync(IdentityMessage message)
        {
            //We should implement sending through dConfig Twillo account
            throw new NotImplementedException();
        }

        public async Task SendAsync(IdentityMessage message, int companyContextId)
        {
            var providerName = websiteSettingsAPI.Get(WebsiteSetting.SMS_ProviderName, companyContextId);
            switch (providerName)
            {
                case "Twillio":
                    var accountSid = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_AccountSid, companyContextId);
                    var authToken = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_AuthToken, companyContextId);
                    var twilioFrom = websiteSettingsAPI.Get(WebsiteSetting.SMS_Twilio_From, companyContextId);
                    await SendAsyncUsingTwilio(message, accountSid, authToken, twilioFrom);
                    break;
                case "ASPSMS":
                    var SMSAccountIdentification = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_AccountSid, companyContextId);
                    var SMSAccountPassword = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_AuthToken, companyContextId);
                    var SMSFrom = websiteSettingsAPI.Get(WebsiteSetting.SMS_ASPSMS_From, companyContextId);
                    await SendAsyncUsingASPSMS(message, SMSAccountIdentification, SMSAccountPassword, SMSFrom);
                    break;
                default:
                    await Task.FromResult(0);
                    break;
            }
        }

        public async Task SendAsyncUsingTwilio(IdentityMessage message, string accountSid, string authToken, string twilioFrom)
        {
            TwilioClient.Init(accountSid, authToken);
            var to = new PhoneNumber(message.Destination);
            var result = MessageResource.Create(
                to,
                from: new PhoneNumber(twilioFrom),
                body: message.Body);
            await Task.FromResult(0);
        }

        public async Task SendAsyncUsingASPSMS(IdentityMessage message, string SMSAccountIdentification, string SMSAccountPassword, string SMSFrom)
        {
            var soapSms = new SABFramework.ASPSMSX2.ASPSMSX2SoapClient("ASPSMSX2Soap");
            soapSms.SendSimpleTextSMS(SMSAccountIdentification, SMSAccountPassword, message.Destination, SMSFrom, message.Body);
            soapSms.Close();
            await Task.FromResult(0);
        }

    }
}
