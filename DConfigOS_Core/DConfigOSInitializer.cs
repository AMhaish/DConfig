using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using SABFramework.Core;
using System.IO;
using System.Web.Optimization;
using System.Web.Configuration;
using DConfigOS_Core.Providers.ResourcesProviders;
using DConfigOS_Core.Providers.HttpContextProviders;
using DConfigOS_Core.Providers.DataContexts;
using SABFramework.PreDefinedModules.MembershipModule;
using System.Web;
using Ninject;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace DConfigOS_Core
{
    public class DConfigOSInitializer : IInitializer
    {
        //public static bool OSinDebugMode = false;
        public void Initialize()
        {
            //Database.SetInitializer(new SABFramework.ModulesUtilities.CreateTablesOnlyIfTheyDontExist<DConfigOS_Core_DBContext>());
            InitializePublicWebsite();
            //Initialize resources folder
            if (!Directory.Exists(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources"))
            {
                Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources");
            }
            BundleTable.EnableOptimizations = false;
        }

        protected void InitializePublicWebsite()
        {
            //CompilationSection section = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
            //OSinDebugMode = section != null && section.Debug;
            BundlesProvider.Instance.InitializeStylesBundles();
            BundlesProvider.Instance.InitializeScriptsBundles();
            //SitemapProvider.Instance.InitializeSitemap();
            DConfigRequestContext.InitializeDomains();
            //DConfigOS_Core_ElasticSearchContext.InitializeIndexes();
            SABPreActionEventsRegistrar.Instance.RegisterRegularEvent(new SABPreActionEventsRegistrar.AuthEvent(ContextSetter));
            SABPostActionEventsRegistrar.Instance.RegisterRegularEvent(new SABPostActionEventsRegistrar.RegularPostEvent(WebhooksCaller));
        }

        private void WebhooksCaller(HttpContextBase httpContext, SABActionInfo actionInfo, IAction actionModel, out string message)
        {
            message = null;
            if (DConfigRequestContext.Current.ContextId.HasValue || MembershipProvider.Instance.ContextCompanyId.HasValue)
            {
                HttpClient client = new HttpClient();
                var webhooksProvider = SABCoreEngine.Instance.DInjector.Get<IWebhookAPI>();
                var requestWebhooks = webhooksProvider.GetRequestWebhooks(actionInfo.ControllerName, actionInfo.ActionName, SABActionInfo.RequestTypeToString(actionInfo.RequestType), (DConfigRequestContext.Current.ContextId.HasValue ? DConfigRequestContext.Current.ContextId.Value : MembershipProvider.Instance.ContextCompanyId.Value));
                foreach (var webhook in requestWebhooks)
                {
                    try
                    {
                        client.PostAsync(webhook.WebhookUrl, new StringContent(JsonConvert.SerializeObject(actionModel, Formatting.None),Encoding.UTF8, "application/json"));
                    }
                    catch (Exception ex)
                    {
                        message = ex.Message;
                    }
                }
            }
        }

        protected bool ContextSetter(System.Web.HttpContextBase httpContext, out string message)
        {
            message = "";
            if (httpContext.User.Identity.IsAuthenticated)
            {
                int contextId;
                if (int.TryParse(httpContext.Request.Headers["ContextId"], out contextId))
                {
                    MembershipProvider.Instance.ContextCompanyId = contextId;
                    return true;
                }
            }
            var currentDomain = httpContext.Request.Url.Host;
            if (currentDomain == "localhost")
            {
                currentDomain = SABCoreEngine.Instance.Settings["Domain"];
            }
            if (currentDomain != "dconfig.com" && DConfigRequestContext.Domains.ContainsKey(currentDomain))
            {
                var context = DConfigRequestContext.Contexts[currentDomain];
                MembershipProvider.Instance.ContextCompanyId = context;
            }
            return true;
        }
    }
}
