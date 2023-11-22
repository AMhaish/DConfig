using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace DConfigOS_Core.Providers.HttpContextProviders
{
    public class DConfigRequestContext
    {
        private IWebsiteSettingsAPI websiteSettingsAPI;
        public DConfigRequestContext(IWebsiteSettingsAPI websiteSettingsAPI)
        {
            this.websiteSettingsAPI = websiteSettingsAPI;
        }

        private string _Language;
        private string _PrecedingLanguage;
        public string Language
        {
            get
            {
                if (String.IsNullOrEmpty(_Language))
                {
                    _Language = websiteSettingsAPI.Get(WebsiteSetting.Language_PublicDefaultLanguage, ContextId);
                }
                return _Language;
            }
            set
            {
                _PrecedingLanguage = _Language;
                _Language = value;
            }
        }
        public int? Version { get; set; }
        public int? DomainId { get; set; }
        public int? ContextId { get; set; }
        private static Dictionary<string, int> _Domains;
        private static Dictionary<string, int?> _Contexts;
        public static Dictionary<string, int> Domains
        {
            get
            {
                if (_Domains == null)
                {
                    InitializeDomains();
                }
                return _Domains;
            }
        }
        public static Dictionary<string, int?> Contexts
        {
            get
            {
                if (_Contexts == null)
                {
                    InitializeDomains();
                }
                return _Contexts;
            }
        }

        //While the current context is coming from the session, in that case one user has access to it according to his sesion
        public static DConfigRequestContext Current
        {
            get
            {
                if (HttpContext.Current.Session != null)
                {
                    DConfigRequestContext context = (DConfigRequestContext)HttpContext.Current.Session["DConfigRequestContext"];
                    if (context == null)
                    {
                        context = new DConfigRequestContext(
                            SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IWebsiteSettingsAPI>()
                            );
                        HttpContext.Current.Session.Add("DConfigRequestContext", context);
                    }
                    return context;
                }
                else
                {
                    return null;
                }
            }
        }

        public static void InitializeDomains()
        {
            _Domains = new Dictionary<string, int>();
            _Contexts = new Dictionary<string, int?>();
            _Domains.Clear();
            List<Domain> domains = SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IContentsAPI>().GetDomains(null, true);
            foreach (Domain d in domains)
            {
                string[] aliases = d.DomainAliases.TrimStart(';').TrimEnd(';').Split(';');
                foreach (string s in aliases)
                {
                    if (!_Domains.ContainsKey(s))
                        _Domains.Add(s, d.Id);
                    if (!_Contexts.ContainsKey(s))
                        _Contexts.Add(s, d.ContextCompanyId);
                }
            }
        }

        public void ResetPrecedingLanguage()
        {
            _Language = _PrecedingLanguage;
        }

        private DConfigRequestContext() { }
    }
}
