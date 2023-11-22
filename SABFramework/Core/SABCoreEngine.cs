using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;
using SABFramework.Core.DataCore;
using SABFramework.Core;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Collections;
using System.Web.Configuration;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Xml.Schema;
using System.Xml.Linq;
using SABFramework.Core.Exceptions;
using Ninject;
using SABFramework.Providers;

namespace SABFramework.Core
{
    public class SABCoreEngine
    {
        //Private Variables
        private DataCore.SABConfig DataContext;

        //Public Properties
        public string AppPhysicalPath { get; set; }
        public StandardKernel DInjector { get; set; }

        //Getters
        public SABSettings Settings { get; private set; }
        public Dictionary<string, string> InMemorySettings { get; private set; }
        public SABModulesProxy ModulesProxy { get; private set; }
        public SABErrorHandler ErrorHandler { get; set; }

        private static SABCoreEngine _instance;
        public static SABCoreEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SABCoreEngine();
                }
                return _instance;
            }
        }

        private SABCoreEngine() {
            InMemorySettings = new Dictionary<string, string>();
        }

        //Procedures
        internal bool ValidateConfigFile(XDocument xmlFrag, string schemaName, string generalErrorMessage = null)
        {
            bool errors = false;
            XmlSchemaSet schema = new XmlSchemaSet();
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("SABFramework." + schemaName))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(stream);
                try
                {
                    schema.Add("", reader);
                    xmlFrag.Validate(schema, (o, e) =>
                    {
                        ErrorHandler.ProcessError((generalErrorMessage ?? "") + e.Message);
                        errors = true;
                    });
                    return !errors;
                }
                catch (XmlException XmlExp)
                {
                    ErrorHandler.ProcessError((generalErrorMessage ?? ""), XmlExp);
                    return false;
                }
                catch (XmlSchemaException XmlSchExp)
                {
                    ErrorHandler.ProcessError((generalErrorMessage ?? ""), XmlSchExp);
                    return false;
                }
                catch (Exception GenExp)
                {
                    ErrorHandler.ProcessError((generalErrorMessage ?? ""), GenExp);
                    return false;
                }
            }
        }

        public void StartEngine(string physicalPath = null)
        {
            DInjector = new StandardKernel();
            if (!String.IsNullOrEmpty(physicalPath))
            {
                AppPhysicalPath = physicalPath;
            }
            else
            {
                AppPhysicalPath = HttpContext.Current.Server.MapPath("~").TrimEnd('\\');
            }
            DInjector.Load(Assembly.GetExecutingAssembly());
            var emailProvider = DInjector.Get<IEmailProvider>();
            ErrorHandler = new SABErrorHandler(emailProvider, AppPhysicalPath);
            InitializeConfiguration();
            ControllerBuilder.Current.SetControllerFactory(typeof(SABActionControllerFactory));
            ModelBinders.Binders.DefaultBinder = new SABModelBinder();
        }

        public void InitializeConfiguration()
        {
            string result;
            XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
            var assembly = Assembly.GetExecutingAssembly();
            XDocument basicXmlFile = XDocument.Load(assembly.GetManifestResourceStream("SABFramework.SAB.config"));
            DataContext = SABConfig.DeSerialize(basicXmlFile.ToString());
            if (ValidateConfigFile(xmlFile, "SABConfigSchema.xsd", "Error in SABConfig file: "))
            {
                result = xmlFile.ToString();
                var newDataContext = SABConfig.DeSerialize(result);
                foreach (DataCore.Module m in newDataContext.Modules)
                {
                    DataContext.Modules.Add(m);
                }
                foreach (DataCore.SettingsRecord s in newDataContext.Settings)
                {
                    DataContext.Settings.Add(s);
                }
                foreach (DataCore.Route r in newDataContext.Routes)
                {
                    DataContext.Routes.Add(r);
                }
            }
            else
            {
                return;
            }
            Settings = new SABSettings(DataContext.Settings);
            ModulesProxy = new SABModulesProxy(DataContext);
            System.Net.Http.Formatting.JsonMediaTypeFormatter jsonMediaTypeFormatter = System.Web.Http.GlobalConfiguration.Configuration.Formatters.OfType<System.Net.Http.Formatting.JsonMediaTypeFormatter>().FirstOrDefault();
            jsonMediaTypeFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        }

        public void LoadModule(string nameSpace, string path, string configPath, string viewsPath, bool _override = false, bool builtIn = false)
        {
            XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
            var modulesSec = xmlFile.Element("sabConfig").Element("modules");
            var newModule = new XElement("module", new XAttribute("namespace", nameSpace), new XAttribute("path", path), new XAttribute("configPath", configPath), new XAttribute("viewsPath", viewsPath));
            modulesSec.Add(newModule);
            xmlFile.Save(AppPhysicalPath + "\\SAB.config");
            DataCore.Module m = new DataCore.Module();
            m.Namespace = nameSpace;
            m.Path = path;
            m.ViewsPath = viewsPath;
            m.ConfigPath = configPath;
            m.Override = _override;
            m.BuiltIn = builtIn;
            DataContext.Modules.Add(m);
            ModulesProxy.LoadModule(m);
        }

        public void UnloadModule(string nameSpace)
        {
            var module = DataContext.Modules.SingleOrDefault(m => m.Namespace == nameSpace);
            if (module != null)
            {
                ModulesProxy.UnloadModule(nameSpace);
                DataContext.Modules.Remove(module);
                XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
                var modulesSec = xmlFile.Element("sabConfig").Element("modules");
                var distModule = modulesSec.Elements().SingleOrDefault(m => m.Attribute("namespace").Value == nameSpace);
                if (distModule != null)
                {
                    distModule.Remove();
                }
                xmlFile.Save(AppPhysicalPath + "\\SAB.config");
            }
        }

        public void AddRouteToConfigFile(string key, string pattern, List<RouteDefaultValue> routeDefaultValues)
        {
            XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
            var modulesSec = xmlFile.Element("sabConfig").Element("routes");
            var defaultValues = new XElement("defaults");
            foreach (RouteDefaultValue r in routeDefaultValues)
            {
                defaultValues.Add(new XElement("default"), new XAttribute("key", r.key), new XAttribute("value", r.value), new XAttribute("optional", r.optional));
            }
            var queryObj = modulesSec.Descendants("route").Where(m => m.Attribute("key").Value == key).FirstOrDefault();
            if (queryObj != null)
            {
                queryObj.Value = pattern;
                queryObj.Descendants("defaults").Remove();
                queryObj.ReplaceNodes(defaultValues);
            }
            else
            {
                var newModule = new XElement("route", new XAttribute("key", key), new XAttribute("pattern", pattern), defaultValues);
                modulesSec.Add(newModule);
            }
            xmlFile.Save(AppPhysicalPath + "\\SAB.config");
        }
        public struct RouteDefaultValue
        {
            public string key;
            public string value;
            public bool optional;
        }
        public void UpdateSettingsRecordInConfigFile(string key, string value)
        {
            XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
            var settingsSec = xmlFile.Element("sabConfig").Element("settings");
            var queryObj = settingsSec.Descendants("settingsRecord").Where(m => m.Attribute("key").Value == key).FirstOrDefault();
            if (queryObj != null)
            {
                queryObj.SetAttributeValue("value", value);
            }
            else
            {
                var newSettings = new XElement("settingsRecord", new XAttribute("key", key), new XAttribute("value", value));
                settingsSec.Add(newSettings);
            }
            xmlFile.Save(AppPhysicalPath + "\\SAB.config");
            Settings.UpdateKey(key, value);
        }

        public void UpdateMultipleSettingsRecordInConfigFile(List<KeyValuePair<string, string>> KeyValuesPairs)
        {
            XDocument xmlFile = XDocument.Load(AppPhysicalPath + "\\SAB.config");
            var settingsSec = xmlFile.Element("sabConfig").Element("settings");
            foreach (KeyValuePair<string, string> kv in KeyValuesPairs)
            {
                var key = kv.Key;
                var value = (String.IsNullOrEmpty(kv.Value) ? "" : kv.Value);
                var queryObj = settingsSec.Descendants("settingsRecord").Where(m => m.Attribute("key").Value == key).FirstOrDefault();
                if (queryObj != null)
                {
                    queryObj.SetAttributeValue("value", value);
                }
                else
                {
                    var newSettings = new XElement("settingsRecord", new XAttribute("key", key), new XAttribute("value", value));
                    settingsSec.Add(newSettings);
                }
                Settings.UpdateKey(key, value);
            }
            xmlFile.Save(AppPhysicalPath + "\\SAB.config");
        }

        public SABSettings GetModuleSettings(string moduleName)
        {
            return ModulesProxy.GetModuleSettings(moduleName);
        }
    }
}