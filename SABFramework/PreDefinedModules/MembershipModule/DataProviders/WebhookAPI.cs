using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public class WebhookAPI : IWebhookAPI
    {
        public virtual List<Webhook> GetRequestWebhooks(string controller, string action, string requestType, int contextCompanyId)
        {
            MembershipDBContext context = new MembershipDBContext();
            var webhooks = context.Webhooks.Where(m => m.ContextCompanyId == contextCompanyId && m.Controller == controller && m.Action == action && m.RequestType == requestType).ToList();
            return webhooks;
        }
    }
}
