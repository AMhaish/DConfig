using SABFramework.PreDefinedModules.MembershipModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public interface IWebhookAPI
    {
        List<Webhook> GetRequestWebhooks(string controller, string action, string RequestType, int contextCompanyId);

    }
}
