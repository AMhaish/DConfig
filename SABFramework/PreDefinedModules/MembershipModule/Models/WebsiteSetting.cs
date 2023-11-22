using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SABFramework.PreDefinedModules.MembershipModule.Models
{
    public class WebsiteSetting : SABFramework.PreDefinedModules.MembershipModule.Models.SABCoreEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public const string General_Domain = "General_Domain";
        public const string General_ErrorPath = "General_ErrorPath";
        public const string General_LoginUrl = "General_LoginUrl";
        public const string General_LogoutAction = "General_LogoutAction";
        public const string General_LogoutRedirect = "General_LogoutRedirect";
        public const string General_NotFoundPath = "General_NotFoundPath";
        public const string General_ResetPasswordPath = "General_ResetPasswordPath";
        public const string General_ForgetpasswordEmailTemplate = "General_ForgetpasswordEmailTemplate";
        public const string General_DConfigIsStartPage = "General_DConfigIsStartPage";

        public const string Language_PublicDefaultLanguage = "Language_PublicDefaultLanguage";
        public const string Language_PortalLanguage = "Language_PortalLanguage";

        public const string SMTP_From = "SMTP_From";
        public const string SMTP_Host = "SMTP_Host";
        public const string SMTP_Port = "SMTP_Port";
        public const string SMTP_UserName = "SMTP_UserName";
        public const string SMTP_Password = "SMTP_Password";
        public const string SMTP_SSL= "SMTP_SSL";

        public const string Security_EmailRegConfirmation = "Security_EmailRegConfirmation";
        public const string Security_SMSRegConfirmation = "Security_SMSRegConfirmation";
        public const string Security_MicrosoftClientId = "Security_MicrosoftClientId";
        public const string Security_MicrosoftClientSecret = "Security_MicrosoftClientSecret";
        public const string Security_TwitterClientId = "Security_TwitterClientId";
        public const string Security_TwitterClientSecret = "Security_TwitterClientSecret";
        public const string Security_FacebookClientId = "Security_FacebookClientId";
        public const string Security_FacebookClientSecret = "Security_FacebookClientSecret";
        public const string Security_GoogleClientId = "Security_GoogleClientId";
        public const string Security_GoogleClientSecret = "Security_GoogleClientSecret";

        public const string SMS_ProviderName = "SMS_ProviderName";
        public const string SMS_Twilio_AccountSid = "SMS_Twilio_AccountSid";
        public const string SMS_Twilio_AuthToken = "SMS_Twilio_AuthToken";
        public const string SMS_Twilio_From = "SMS_Twilio_From";
        public const string SMS_ASPSMS_AccountSid = "SMS_ASPSMS_AccountSid";
        public const string SMS_ASPSMS_AuthToken = "SMS_ASPSMS_AuthToken";
        public const string SMS_ASPSMS_From = "SMS_ASPSMS_From";
    }
}
