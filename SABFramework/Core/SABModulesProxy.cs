using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SABFramework.Core.DataCore;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml;
using System.Dynamic;
using Ninject;

namespace SABFramework.Core
{
    public class SABModulesProxy
    {
        private SABConfig _configData;
        private Dictionary<string, SABFramework.Core.DataCore.Module> _modulesInfo;
        private Dictionary<string, ModuleConfig> _modulesConfigs;
        private Dictionary<string, DataCore.Area> _actionsTreeWithAreas;
        private Dictionary<string, DataCore.Controller> _actionsTree;
        private Dictionary<string, ModuleRemoteManager> _modulesRemoteManager;
        private Dictionary<string, AppDomain> _modulesAppDomains;
        private Dictionary<string, SABSettings> _modulesSettings;

        public IEnumerable<DataCore.Controller> RegisteredControllers { get { return _actionsTree.Values; } }
        public IEnumerable<DataCore.Area> RegisteredAreas { get { return _actionsTreeWithAreas.Values; } }
        internal SABModulesProxy(SABConfig configData)
        {
            _configData = configData;
            _modulesConfigs = new Dictionary<string, ModuleConfig>();
            _actionsTreeWithAreas = new Dictionary<string, DataCore.Area>();
            _actionsTree = new Dictionary<string, DataCore.Controller>();
            _modulesRemoteManager = new Dictionary<string, ModuleRemoteManager>();
            _modulesAppDomains = new Dictionary<string, AppDomain>();
            _modulesInfo = new Dictionary<string, DataCore.Module>();
            _modulesSettings = new Dictionary<string, SABSettings>();
            //Register module routing should be before global routing
            foreach (SABFramework.Core.DataCore.Module m in _configData.Modules)
            {
                LoadModule(m);
            }
            RegisterRoutes(_configData.Routes, RouteTable.Routes);
        }

        private void DoModuleInitialization(SABFramework.Core.DataCore.Module module, SABFramework.Core.DataCore.ModuleConfig moduleConfig)
        {
            Type initializerType;
            IInitializer initializer = null;
            Assembly assembly;
            if (!String.IsNullOrEmpty(moduleConfig.Initializer))
            {
                if (module.BuiltIn)
                {
                    initializerType = Type.GetType(module.Namespace + "." + moduleConfig.Initializer);
                    initializer = (IInitializer)Activator.CreateInstance(initializerType);
                }
                else
                {
                    try
                    {
                        assembly = Assembly.LoadFrom(module.PhysicalPath);
                        initializerType = assembly.GetType(module.Namespace + "." + moduleConfig.Initializer);
                        initializer = (IInitializer)Activator.CreateInstance(initializerType);
                        // Loading DI rules for Ninject
                        SABCoreEngine.Instance.DInjector.Load(assembly);
                    }
                    catch (System.IO.FileNotFoundException ex)
                    {
                        SABCoreEngine.Instance.ErrorHandler.ProcessError(" (Can't find dll in the path \"" + module.PhysicalPath + "\")", ex);
                    }
                    catch (BadImageFormatException ex2)
                    {
                        SABCoreEngine.Instance.ErrorHandler.ProcessError(" (The Dll in the path \"" + module.PhysicalPath + "\" is corrupted)", ex2);
                    }
                    catch (System.Security.SecurityException ex3)
                    {
                        SABCoreEngine.Instance.ErrorHandler.ProcessError(" (The Application doesn't have permissions to access the Dll in the path \"" + module.PhysicalPath + "\")", ex3);
                    }
                    catch (ArgumentException ex3)
                    {
                        SABCoreEngine.Instance.ErrorHandler.ProcessError(" (Couldn't find the type " + module.PhysicalPath + " in the dll)", ex3);
                    }
                    catch (InvalidCastException ex4)
                    {
                        SABCoreEngine.Instance.ErrorHandler.ProcessError("(Couldn't cast the action into IAction)", ex4);
                    }
                }
                if (initializer != null)
                {
                    initializer.Initialize();
                }
            }
        }

        internal DataCore.Action GetActionDescriptor(string areaName, string controllerName, string actionName, DataCore.RequestType requestType)
        {
            if (!String.IsNullOrEmpty(areaName))
            {
                if (_actionsTreeWithAreas.ContainsKey(areaName))
                {
                    var area = _actionsTreeWithAreas[areaName];
                    if (area.ControllersDic.ContainsKey(controllerName))
                    {
                        var controller = area.ControllersDic[controllerName];
                        if (controller.ActionsDic.ContainsKey(actionName + requestType))
                        {
                            return controller.ActionsDic[actionName + requestType];
                        }
                    }
                }
            }
            else
            {
                if (_actionsTree.ContainsKey(controllerName))
                {
                    var controller = _actionsTree[controllerName];
                    if (controller.ActionsDic.ContainsKey(actionName + requestType))
                    {
                        return controller.ActionsDic[actionName + requestType];
                    }
                }
            }
            return null;
        }

        private void RegisterRoutes(IEnumerable<DataCore.Route> routeToBeRegistered, RouteCollection routes)
        {
            foreach (DataCore.Route r in routeToBeRegistered)
            {
                var defaults = new RouteValueDictionary();
                foreach (Default d in r.Defaults)
                {
                    if (d.Optional)
                        defaults.Add(d.Key, UrlParameter.Optional);
                    else
                        defaults.Add(d.Key, d.Value);
                }
                //var defaultsObj = new DynObj(defaults);
                routes.Add(new System.Web.Routing.Route(r.Pattern, defaults, new MvcRouteHandler()));
                //routes.MapRoute(
                //        r.Key, // Route name
                //        r.Pattern, // URL with parameters
                //        defaultsObj // Parameter defaults
                //    );
            }
        }

        //internal sealed class DynObj : DynamicObject
        //{
        //    private readonly Dictionary<string, object> _properties;

        //    public DynObj(Dictionary<string, object> properties)
        //    {
        //        _properties = properties;
        //    }

        //    public override IEnumerable<string> GetDynamicMemberNames()
        //    {
        //        return _properties.Keys;
        //    }

        //    public override bool TryGetMember(GetMemberBinder binder, out object result)
        //    {
        //        if (_properties.ContainsKey(binder.Name))
        //        {
        //            result = _properties[binder.Name];
        //            return true;
        //        }
        //        else
        //        {
        //            result = null;
        //            return false;
        //        }
        //    }

        //    public override bool TrySetMember(SetMemberBinder binder, object value)
        //    {
        //        if (_properties.ContainsKey(binder.Name))
        //        {
        //            _properties[binder.Name] = value;
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //}

        public ActionResult InvokeAction(string areaName, string controllerName, string actionName, DataCore.RequestType requestType)
        {
            //Need to be done
            throw new NotImplementedException();
        }

        internal void LoadModule(SABFramework.Core.DataCore.Module m)
        {
            string result;
            string moduleName = m.Namespace;
            if (!_modulesInfo.ContainsKey(m.Namespace))
            {
                _modulesInfo.Add(m.Namespace, m);
                XDocument xmlFile = null;
                if (m.BuiltIn)
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    xmlFile = XDocument.Load(assembly.GetManifestResourceStream("SABFramework." + m.ConfigPath));
                }
                else
                {
                    xmlFile = XDocument.Load(SABCoreEngine.Instance.AppPhysicalPath + m.ConfigPath.Replace("/", "\\"));
                    m.PhysicalPath = SABCoreEngine.Instance.AppPhysicalPath + m.Path.Replace("/", "\\");
                    if (!_modulesRemoteManager.ContainsKey(moduleName))
                    {
                        AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
                        AppDomain newDomain = AppDomain.CreateDomain(moduleName + "AppDomain", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain 
                        System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(ModuleRemoteManager).Assembly.FullName, typeof(ModuleRemoteManager).FullName);
                        ModuleRemoteManager manager = (ModuleRemoteManager)obj.Unwrap();
                        manager.LoadAssembly(moduleName, m.PhysicalPath);
                        _modulesRemoteManager.Add(moduleName, manager);
                        _modulesAppDomains.Add(moduleName, newDomain);
                    }
                }
                if (SABCoreEngine.Instance.ValidateConfigFile(xmlFile, "ModuleConfigSchema.xsd", "Error in ModuleConfig file, module name (" + m.Namespace + "), error: "))
                {
                    result = xmlFile.ToString();
                    var dataContext = ModuleConfig.DeSerialize(result);
                    _modulesConfigs.Add(m.Namespace, dataContext);
                    foreach (DataCore.Controller c in dataContext.Controllers)
                    {
                        c.ControllerModule = m;
                        if (!_actionsTree.ContainsKey(c.Name))
                            _actionsTree.Add(c.Name, c);
                        if (_actionsTree.ContainsKey(c.Name) && m.Override)
                        {
                            DataCore.Controller currentController;
                            foreach (KeyValuePair<string, DataCore.Action> kv in c.ActionsDic)
                            {
                                currentController = _actionsTree[c.Name];
                                if (currentController.ActionsDic.ContainsKey(kv.Key))
                                {
                                    currentController.ActionsDic[kv.Key] = kv.Value;
                                }
                                else
                                {
                                    currentController.ActionsDic.Add(kv.Key, kv.Value);
                                }
                            }
                        }
                    }
                    foreach (Area a in dataContext.Areas)
                    {
                        a.AreaModule = m;
                        if (!_actionsTree.ContainsKey(a.Name) || _actionsTree.ContainsKey(a.Name) && m.Override)
                            _actionsTreeWithAreas.Add(a.Name, a);
                        if (_actionsTree.ContainsKey(a.Name) && m.Override)
                        {
                            DataCore.Controller currentController;
                            foreach (DataCore.Controller c in a.Controllers)
                            {
                                foreach (KeyValuePair<string, DataCore.Action> kv in c.ActionsDic)
                                {
                                    currentController = _actionsTree[c.Name];
                                    if (currentController.ActionsDic.ContainsKey(kv.Key))
                                    {
                                        currentController.ActionsDic[kv.Key] = kv.Value;
                                    }
                                    else
                                    {
                                        currentController.ActionsDic.Add(kv.Key, kv.Value);
                                    }
                                }
                            }
                        }
                    }
                    _modulesSettings.Add(m.Namespace, new SABSettings(dataContext.Settings));
                    DoModuleInitialization(m, dataContext);
                    RegisterRoutes(dataContext.Routes, RouteTable.Routes);
                }
            }
        }

        internal void UnloadModule(string moduleName)
        {
            if (_modulesRemoteManager.ContainsKey(moduleName))
            {
                _modulesInfo.Remove(moduleName);
                _modulesConfigs.Remove(moduleName);
                _modulesRemoteManager.Remove(moduleName);
                AppDomain.Unload(_modulesAppDomains[moduleName]);
                _modulesAppDomains.Remove(moduleName);
                var areasKeys = _actionsTreeWithAreas.Where(m => m.Value.AreaModule.Namespace == moduleName).Select(m => m.Key);
                foreach (string areaKey in areasKeys)
                {
                    _actionsTreeWithAreas.Remove(areaKey);
                }
                var controllersKeys = _actionsTree.Where(m => m.Value.ControllerModule.Namespace == moduleName).Select(m => m.Key);
                foreach (string controllerKey in controllersKeys)
                {
                    _actionsTree.Remove(controllerKey);
                }
            }
        }

        public Type GetTypeFromModule(string moduleName, string typeName)
        {
            if (_modulesInfo.ContainsKey(moduleName))
            {
                if (_modulesInfo[moduleName].BuiltIn)
                {
                    return Type.GetType(moduleName + "." + typeName);
                }
                else
                {
                    if (_modulesRemoteManager.ContainsKey(moduleName))
                    {
                        var manager = _modulesRemoteManager[moduleName];
                        return manager.GetClassType(typeName);
                    }
                }
            }
            return null;
        }

        public SABSettings GetModuleSettings(string moduleName)
        {
            if (_modulesSettings.ContainsKey(moduleName))
            {
                return _modulesSettings[moduleName];
            }
            return null;
        }
        class ModuleRemoteManager : MarshalByRefObject
        {
            private string _moduleName;
            private Assembly _assembly;
            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void LoadAssembly(string moduleName, string path)
            {
                _assembly = System.Reflection.Assembly.LoadFrom(path);
                _moduleName = moduleName;
            }

            public Type GetClassType(string TypeName)
            {
                return _assembly.GetType(_moduleName + "." + TypeName);
            }
        }
    }
}
