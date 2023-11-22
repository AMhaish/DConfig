using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.Providers;
using SABFramework.Core;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;

namespace SABFramework
{
    public class DIInitializer : NinjectModule
    {
        public override void Load()
        {
            Bind<IEmailProvider>().To<EmailProvider>();
            Bind<ISMSProvider>().To<SMSProvider>();
            Bind<ICompaniesAPI>().To<CompaniesAPI>();
            Bind<IPrivilegeAPI>().To<PrivilegeAPI>();
            Bind<IUsersAPI>().To<UsersAPI>();
            Bind<IWebsiteSettingsAPI>().To<WebsiteSettingsAPI>();
            Bind<IWebhookAPI>().To<WebhookAPI>();
        }
    }
}
