using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using Ninject;

namespace DConfigOS_Core.Providers.ResourcesProviders
{
    public class BundlesProvider
    {
        private IStylesBundlesAPI stylesBundleAPI;
        private IScriptsBundlesAPI scriptsBundleAPI;
        public BundlesProvider(IStylesBundlesAPI stylesBundleAPI, IScriptsBundlesAPI scriptsBundleAPI) {
            this.stylesBundleAPI = stylesBundleAPI;
            this.scriptsBundleAPI = scriptsBundleAPI;
        }

        private static BundlesProvider _instance;
        public static BundlesProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BundlesProvider(
                        SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IStylesBundlesAPI>(),
                        SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IScriptsBundlesAPI>()
                        );
                }
                return _instance;
            }
        }

        public void InitializeStylesBundles()
        {
            var stylesBundles = stylesBundleAPI.GetStylesBundles();
            foreach (StylesBundle s in stylesBundles)
            {
                Bundle sb = null;
                //if (OSinDebugMode)
                //{
                //sb = new Bundle("~/Content/" + s.Name + s.ContextCompanyId.ToString());
                //}
                //else
                //{
                sb = new StyleBundle("~/Content/" + s.Name + s.ContextCompanyId.ToString());
                //}
                if (!BundleTable.Bundles.Contains(sb))
                {
                    string[] scriptspath = s.Styles.OrderBy(m => m.Priority).Select(m => m.Path).ToArray();
                    sb.Include(scriptspath);
                    BundleTable.Bundles.Add(sb);

                }
            }
        }

        public void InitializeScriptsBundles()
        {
            var scriptsBundles = scriptsBundleAPI.GetScriptsBundles();
            foreach (ScriptsBundle s in scriptsBundles)
            {
                Bundle sb = null;
                //if (OSinDebugMode)
                //{
                //sb = new Bundle("~/bundles/" + s.Name + s.ContextCompanyId.ToString());
                //}
                //else
                //{
                sb = new ScriptBundle("~/bundles/" + s.Name + s.ContextCompanyId.ToString());
                //}
                if (!BundleTable.Bundles.Contains(sb))
                {
                    string[] scriptspath = s.Scripts.OrderBy(m => m.Priority).Select(m => m.Path).ToArray();
                    sb.Include(scriptspath);
                    BundleTable.Bundles.Add(sb);
                }
            }
        }

        public void ReInitializeStyleBundle(StylesBundle styleBundle)
        {
            RemoveStyleBundle(styleBundle);
            var newBundle = new StyleBundle("~/Content/" + styleBundle.Name + styleBundle.ContextCompanyId.ToString());
            newBundle.Include(styleBundle.Styles.OrderBy(m => m.Priority).Select(m => m.Path).ToArray());
            BundleTable.Bundles.Add(newBundle);
        }

        public void ReInitializeScriptBundle(ScriptsBundle scriptBundle)
        {
            RemoveScriptBundle(scriptBundle);
            var newBundle = new ScriptBundle("~/bundles/" + scriptBundle.Name + scriptBundle.ContextCompanyId.ToString());
            newBundle.Include(scriptBundle.Scripts.OrderBy(m => m.Priority).Select(m => m.Path).ToArray());
            BundleTable.Bundles.Add(newBundle);
        }

        public void RemoveStyleBundle(StylesBundle styleBundle)
        {
            var oldBundle = new StyleBundle("~/Content/" + styleBundle.Name + styleBundle.ContextCompanyId.ToString());
            BundleTable.Bundles.Remove(oldBundle);
        }

        public void RemoveScriptBundle(ScriptsBundle scriptBundle)
        {
            var oldBundle = new ScriptBundle("~/bundles/" + scriptBundle.Name + scriptBundle.ContextCompanyId.ToString());
            BundleTable.Bundles.Remove(oldBundle);
        }
    }
}
