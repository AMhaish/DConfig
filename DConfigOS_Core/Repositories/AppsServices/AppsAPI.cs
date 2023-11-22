using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;
using System.Net.Http;
using Newtonsoft.Json;
using DConfigOS_Core.Repositories.AppsServices;
namespace DConfigOS_Core.Repositories.AppsServices
{
    public static class HttpContentExtensions
    {
        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            string pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
            {
                throw new InvalidOperationException(string.Format("File {0} already exists.", pathname));
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content.CopyToAsync(fileStream).ContinueWith(
                     (copyTask) =>
                     {
                         fileStream.Close();
                     });
            }
            catch
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }

                throw;
            }
        }
    }

    public class AppsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IAppAPIRepository
    {
        public virtual App GetAppInfo(string appName)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var app = context.Apps.Where(m => m.Name == appName).FirstOrDefault();
            return app;
        }

        public virtual List<App> GetInstalledApps(List<string> roles = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            List<App> appList;
            if (roles == null)
            {
                appList = context.Apps.OrderBy(m => m.Name).ToList();
            }
            else
            {
                appList = context.Apps.Where(m => m.AppRoles.Where(n => roles.Contains(n.Id)).Count() > 0).OrderBy(m => m.Name).ToList();
            }
            foreach (App app in appList)
            {
                app.DisplayName = Regex.Replace(app.Name, "([a-z])_?([A-Z])", "$1 $2");
            }
            return appList;
        }

        public virtual int UpdateAppRoles(string appName,List<string> roles)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var app = context.Apps.SingleOrDefault(m => m.Name == appName);
            if (app != null)
            {
                app.AppRoles.Clear();
                if (roles != null && roles.Count > 0)
                {
                    var dbRoles = context.Roles.Where(m => roles.Contains(m.Id));
                    if (roles != null)
                    {
                        foreach (var role in dbRoles)
                        {
                            app.AppRoles.Add(role);
                        }
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual List<AppClientLogic> GetAppClientLogics(string appName)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.Apps.SingleOrDefault(m => m.Name == appName).AppClientLogics.ToList();
        }

        public virtual List<AppStyleSheet> GetAppStyles(string appName)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.Apps.SingleOrDefault(m => m.Name == appName).AppStyleSheets.ToList();
        }

        public virtual int CheckAppNewUpdates(string appName, out string version)
        {
            version = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://" + SABFramework.Core.SABCoreEngine.Instance.Settings["RemoteUpdateServiceDomain"] + "/DConfig/DConfigModule/checkAppLastVersion?AppName=" + appName));
                    HttpResponseMessage response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
                    var res = response.Content.ReadAsStringAsync().Result;
                    SABFramework.Core.ReturnedTypesHandlers.JsonResult jsonRes = JsonConvert.DeserializeObject<SABFramework.Core.ReturnedTypesHandlers.JsonResult>(res);
                    if (jsonRes.result == "true")
                    {
                        version = (string)jsonRes.obj;
                        return ResultCodes.Succeed;
                    }
                    else
                    {
                        SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(jsonRes.message);
                    }
                }
                return ResultCodes.UnknownError;
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in checking app version", ex);
                return ResultCodes.UnknownError;
            }
        }

        public virtual  int GetAppVersionMetaData(string appName, string version, out FileMetaData metaData)
        {
            metaData = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri("http://" + SABFramework.Core.SABCoreEngine.Instance.Settings["RemoteUpdateServiceDomain"] + "/DConfig/DConfigModule/getAppVersionMetaData?AppName=" + appName + "&Version=" + version));
                    HttpResponseMessage response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
                    var res = response.Content.ReadAsStringAsync().Result;
                    SABFramework.Core.ReturnedTypesHandlers.JsonResult jsonRes = JsonConvert.DeserializeObject<SABFramework.Core.ReturnedTypesHandlers.JsonResult>(res);
                    if (jsonRes.result == "true")
                    {
                        metaData = JsonConvert.DeserializeObject<FileMetaData>(jsonRes.obj.ToString());
                        return ResultCodes.Succeed;
                    }
                    else
                    {
                        SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError(jsonRes.message);
                    }
                }
                return ResultCodes.UnknownError;
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in checking app version", ex);
                return ResultCodes.UnknownError;
            }
        }



        public  int DownloadAppVersionFile(string appName, string version, Guid deviceKey, string userName, string password, out string dapFilePath)
        {
            dapFilePath = null;
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("http://" + SABFramework.Core.SABCoreEngine.Instance.Settings["RemoteUpdateServiceDomain"] + "/DConfig/DConfigModule/downloadAppVersionFile?AppName=" + appName + "&Version=" + version));
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("UserName", userName),
                        new KeyValuePair<string, string>("Password", password),
                        new KeyValuePair<string, string>("DeviceKey", deviceKey.ToString())
                    });
                    request.Content = content;
                    HttpResponseMessage response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string dapPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\App_Data\\packages\\" + appName + "." + version + ".nupkg";
                        var task = response.Content.ReadAsFileAsync(dapPath, true);
                        task.Wait();
                        dapFilePath = dapPath;
                        return ResultCodes.Succeed;
                    }
                    else
                    {
                        return ResultCodes.UnknownError;
                    }
                }

            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in checking app version", ex);
                return ResultCodes.UnknownError;
            }
        }

        public  int InstallApp(string path)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("Setup", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain 
            ComponentsInstaller componentsInstaller = (ComponentsInstaller)newDomain.CreateInstanceAndUnwrap(typeof(ComponentsInstaller).Assembly.FullName, typeof(ComponentsInstaller).FullName);
            componentsInstaller.Initialize(path, SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath);
            string appName = componentsInstaller.GetAppName();
            var oldApp = context.Apps.SingleOrDefault(m => m.Name == appName);
            if (oldApp != null)
            {
                UninstallApp(appName);
            }
            App app = componentsInstaller.InstallAppComponents();
            AppDomain.Unload(newDomain);
            string distPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\bin\\" + app.Name + ".dll";
            File.Copy(path, distPath, true);
            app.DllPath = "/bin/" + app.Name + ".dll";
            app.BuiltInApp = false;
            SABFramework.Core.SABCoreEngine.Instance.LoadModule(app.Name, app.DllPath, app.ConfigPath, "~/Views/AppsViews/" + app.Name);
            var appSettings = SABFramework.Core.SABCoreEngine.Instance.GetModuleSettings(app.Name);
            app.StartPath = "#/" + app.Name;
            //if (appSettings.ContainsKey("StartPath"))
            //    app.StartPath = appSettings["StartPath"];
            context.Apps.Add(app);
            context.SaveChanges();
            return ResultCodes.Succeed; ;
        }
        public  bool UninstallApp(string appName)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var app = context.Apps.SingleOrDefault(m => m.Name == appName);
            if (app != null)
            {
                string appModulePath = app.DllPath;
                var rootPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath;
                foreach (AppView v in app.AppViews)
                {
                    var filePath = rootPath + v.Path.Replace('/', '\\');
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                foreach (AppWidget v in app.AppWidgets)
                {
                    foreach (WidgetView wv in v.WidgetsViews)
                    {
                        var filePath = rootPath + wv.Path.Replace('/', '\\');
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    foreach (WidgetClientLogic wcl in v.WidgetClientLogics)
                    {
                        var filePath = rootPath + wcl.Path.Replace('/', '\\');
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                }
                foreach (AppExtention x in app.AppExtentions)
                {
                    foreach (AppExtentionView xv in x.AppExtViews)
                    {
                        var filePath = rootPath + xv.Path.Replace('/', '\\');
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                    foreach (AppExtentionClientLogic xcl in x.AppExtClientLogics)
                    {
                        var filePath = rootPath + xcl.Path.Replace('/', '\\');
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                    }
                }
                foreach (AppClientLogic v in app.AppClientLogics)
                {
                    var filePath = rootPath + v.Path.Replace('/', '\\');
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                foreach (AppStyleSheet v in app.AppStyleSheets)
                {
                    var filePath = rootPath + v.Path.Replace('/', '\\');
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                if (!String.IsNullOrEmpty(app.ConfigPath))
                {
                    var filePath = rootPath + app.ConfigPath.Replace('/', '\\');
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                context.Apps.Remove(app);
                context.SaveChanges();
                SABFramework.Core.SABCoreEngine.Instance.UnloadModule(appName);
                var dapPath = rootPath + appModulePath.Replace('/', '\\');
                if (File.Exists(dapPath))
                    File.Delete(dapPath);
            }
            return true;
        }
        class ComponentsInstaller : MarshalByRefObject
        {
            private Assembly DAPFile;
            private string rootPath;
            public void Initialize(string path, string rootPath)
            {
                LoadAssembly(path);
                this.rootPath = rootPath;
            }

            public override object InitializeLifetimeService()
            {
                return null;
            }

            public void LoadAssembly(string path)
            {
                DAPFile = System.Reflection.Assembly.ReflectionOnlyLoadFrom(path);
            }

            private enum AppComponenetType { AppView, AppClientLogic, StyleSheet, Config, AppIcon, WidgetComponent, AppExtention }
            private enum WidgetComponentType { WidgetView, WidgetClientLogic }
            private enum ApExtComponentType { ExtView, ExtClientLogic }
            private void InstallAppComponent(string appName, string componentName, Stream componentStream, AppComponenetType componentType, App app, string rootPath)
            {
                StreamReader componentReader = new StreamReader(componentStream);
                StringBuilder distPath = new StringBuilder();
                distPath.Append(rootPath);
                switch (componentType)
                {
                    case AppComponenetType.AppView:
                        distPath.Append("\\Views\\AppsViews\\" + appName);
                        break;
                    case AppComponenetType.WidgetComponent:
                        break;
                    case AppComponenetType.AppClientLogic:
                        distPath.Append("\\AppsClientLogic\\" + appName);
                        break;
                    case AppComponenetType.StyleSheet:
                    case AppComponenetType.AppIcon:
                        distPath.Append("\\Content\\Apps\\" + appName);
                        break;
                    case AppComponenetType.Config:
                        distPath.Append("\\Config");
                        break;
                }
                if (componentType != AppComponenetType.WidgetComponent || componentType != AppComponenetType.AppExtention)
                {
                    if (!Directory.Exists(distPath.ToString()))
                    {
                        Directory.CreateDirectory(distPath.ToString());
                    }
                    distPath.Append("\\");
                    distPath.Append(componentName);
                    StreamWriter writer = new StreamWriter(distPath.ToString());
                    writer.Write(componentReader.ReadToEnd());
                    componentReader.Close();
                    writer.Close();
                }
                switch (componentType)
                {
                    case AppComponenetType.AppView:
                        if (app.AppViews == null)
                            app.AppViews = new List<AppView>();
                        app.AppViews.Add(new AppView() { Name = componentName, Path = "/Views/AppsViews/" + appName + "/" + componentName });
                        return;
                    case AppComponenetType.WidgetComponent:
                        if (app.AppWidgets == null)
                            app.AppWidgets = new List<AppWidget>();
                        var dotIndex = componentName.IndexOf(".");
                        var widgetName = componentName.Substring(0, dotIndex);
                        var namespaceLeft = componentName.Substring(dotIndex + 1, componentName.Length - dotIndex - 1);
                        dotIndex = namespaceLeft.IndexOf(".");
                        var componentTypeString = namespaceLeft.Substring(0, dotIndex);
                        var wcomponentName = namespaceLeft.Substring(dotIndex + 1, namespaceLeft.Length - dotIndex - 1);
                        AppWidget widget = app.AppWidgets.Where(m => m.Name == widgetName).FirstOrDefault();
                        if (widget == null)
                        {
                            widget = new AppWidget() { Name = widgetName };
                            app.AppWidgets.Add(widget);
                        }
                        switch (componentTypeString)
                        {
                            case "Views":
                                InstallWidgetComponent(widgetName, appName, wcomponentName, componentStream, WidgetComponentType.WidgetView, widget, rootPath);
                                break;
                            case "ClientLogic":
                                InstallWidgetComponent(widgetName, appName, wcomponentName, componentStream, WidgetComponentType.WidgetClientLogic, widget, rootPath);
                                break;
                        }
                        return;
                    case AppComponenetType.AppClientLogic:
                        if (app.AppClientLogics == null)
                            app.AppClientLogics = new List<AppClientLogic>();
                        app.AppClientLogics.Add(new AppClientLogic() { Name = componentName, Path = "/AppsClientLogic/" + appName + "/" + componentName });
                        return;
                    case AppComponenetType.AppExtention:
                        if (app.AppExtentions == null)
                            app.AppExtentions = new List<AppExtention>();
                        var extDotIndex = componentName.IndexOf(".");
                        var extName = componentName.Substring(0, extDotIndex);
                        var extNamespaceLeft = componentName.Substring(extDotIndex + 1, componentName.Length - extDotIndex - 1);
                        extDotIndex = extNamespaceLeft.IndexOf(".");
                        var extComponentTypeString = extNamespaceLeft.Substring(0, extDotIndex);
                        var extComponentName = extNamespaceLeft.Substring(extDotIndex + 1, extNamespaceLeft.Length - extDotIndex - 1);
                        AppExtention ext = app.AppExtentions.Where(m => m.Name == extName).FirstOrDefault();
                        if (ext == null)
                        {
                            ext = new AppExtention() { Name = extName };
                            app.AppExtentions.Add(ext);
                        }
                        switch (extComponentTypeString)
                        {
                            case "Views":
                                InstallAppExtComponent(extName, appName, extComponentName, componentStream, ApExtComponentType.ExtView, ext, rootPath);
                                break;
                            case "ClientLogic":
                                InstallAppExtComponent(extName, appName, extComponentName, componentStream, ApExtComponentType.ExtClientLogic, ext, rootPath);
                                break;
                        }
                        return;
                    case AppComponenetType.StyleSheet:
                        if (app.AppStyleSheets == null)
                            app.AppStyleSheets = new List<AppStyleSheet>();
                        app.AppStyleSheets.Add(new AppStyleSheet() { Name = componentName, Path = "/Content/" + appName + "/" + componentName });
                        return;
                    case AppComponenetType.Config:
                        app.ConfigPath = "/Config/" + componentName;
                        return;
                    case AppComponenetType.AppIcon:
                        app.IconPath = "/Content/Apps/" + appName + "/" + componentName;
                        return;
                }
            }

            private void InstallWidgetComponent(string widgetName, string appName, string componentName, Stream componentStream, WidgetComponentType componentType, AppWidget widget, string rootPath)
            {
                StreamReader componentReader = new StreamReader(componentStream);
                StringBuilder distPath = new StringBuilder();
                distPath.Append(rootPath);
                switch (componentType)
                {
                    case WidgetComponentType.WidgetView:
                        distPath.Append("\\Views\\Widgets\\" + appName + "\\" + widgetName);
                        break;
                    case WidgetComponentType.WidgetClientLogic:
                        distPath.Append("\\AppsClientLogic\\" + appName + "\\" + widgetName);
                        break;
                }
                if (!Directory.Exists(distPath.ToString()))
                {
                    Directory.CreateDirectory(distPath.ToString());
                }
                distPath.Append("\\");
                distPath.Append(componentName);
                StreamWriter writer = new StreamWriter(distPath.ToString());
                writer.Write(componentReader.ReadToEnd());
                componentReader.Close();
                writer.Close();
                switch (componentType)
                {
                    case WidgetComponentType.WidgetView:
                        if (widget.WidgetsViews == null)
                            widget.WidgetsViews = new List<WidgetView>();
                        widget.WidgetsViews.Add(new WidgetView()
                        {
                            Name = componentName,
                            Path = "/Views/Widgets/" + appName + "/" + widgetName + "/" + componentName,
                        });
                        return;
                    case WidgetComponentType.WidgetClientLogic:
                        if (widget.WidgetClientLogics == null)
                            widget.WidgetClientLogics = new List<WidgetClientLogic>();
                        widget.WidgetClientLogics.Add(new WidgetClientLogic() { Name = componentName, Path = "/AppsClientLogic/" + appName + "/" + widgetName + "/" + componentName });
                        return;
                }
            }

            private void InstallAppExtComponent(string extentionName, string appName, string componentName, Stream componentStream, ApExtComponentType componentType, AppExtention extention, string rootPath)
            {
                StreamReader componentReader = new StreamReader(componentStream);
                StringBuilder distPath = new StringBuilder();
                distPath.Append(rootPath);
                switch (componentType)
                {
                    case ApExtComponentType.ExtView:
                        distPath.Append("\\Views\\Extentions\\" + appName + "\\" + extentionName);
                        break;
                    case ApExtComponentType.ExtClientLogic:
                        distPath.Append("\\AppsExtentionsLogic\\" + appName + "\\" + extentionName);
                        break;
                }
                if (!Directory.Exists(distPath.ToString()))
                {
                    Directory.CreateDirectory(distPath.ToString());
                }
                distPath.Append("\\");
                distPath.Append(componentName);
                StreamWriter writer = new StreamWriter(distPath.ToString());
                writer.Write(componentReader.ReadToEnd());
                componentReader.Close();
                writer.Close();
                switch (componentType)
                {
                    case ApExtComponentType.ExtView:
                        if (extention.AppExtViews == null)
                            extention.AppExtViews = new List<AppExtentionView>();
                        extention.AppExtViews.Add(new AppExtentionView()
                        {
                            Name = componentName,
                            Path = "/Views/Extentions/" + appName + "/" + extentionName + "/" + componentName,
                        });
                        return;
                    case ApExtComponentType.ExtClientLogic:
                        if (extention.AppExtClientLogics == null)
                            extention.AppExtClientLogics = new List<AppExtentionClientLogic>();
                        extention.AppExtClientLogics.Add(new AppExtentionClientLogic() { Name = componentName, Path = "/AppsClientLogic/" + appName + "/" + extentionName + "/" + componentName });
                        return;
                }
            }

            public App InstallAppComponents()
            {
                App app = new App();
                if (DAPFile != null)
                {
                    app.Name = GetAppName();
                    app.Version = DAPFile.FullName.Substring(DAPFile.FullName.IndexOf("Version=") + 8, 7);
                    app.InstalledDate = DateTime.Now;
                    List<string> resourcesNames = DAPFile.GetManifestResourceNames().ToList();
                    foreach (string r in resourcesNames)
                    {
                        var resourceName = r.Remove(0, app.Name.Length + 1);
                        var resourceStream = DAPFile.GetManifestResourceStream(r);
                        if (resourceName.StartsWith("Views."))
                        {
                            resourceName = resourceName.Remove(0, 6);
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.AppView, app, rootPath);
                        }
                        else if (resourceName.StartsWith("Widgets."))
                        {
                            resourceName = resourceName.Remove(0, 8);
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.WidgetComponent, app, rootPath);
                        }
                        else if (resourceName.StartsWith("ClientLogic."))
                        {
                            resourceName = resourceName.Remove(0, 12);
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.AppClientLogic, app, rootPath);
                        }
                        else if (resourceName.StartsWith("ExtentionLogic."))
                        {
                            resourceName = resourceName.Remove(0, 15);
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.AppExtention, app, rootPath);
                        }
                        else if (resourceName.StartsWith("StyleSheets."))
                        {
                            resourceName = resourceName.Remove(0, 12);
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.StyleSheet, app, rootPath);
                        }
                        else if (resourceName.EndsWith(app.Name + ".config"))
                        {
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.Config, app, rootPath);
                        }
                        else if (resourceName.EndsWith(app.Name + ".png"))
                        {
                            InstallAppComponent(app.Name, resourceName, resourceStream, AppComponenetType.AppIcon, app, rootPath);
                        }
                    }
                    return app;
                }
                return null;
            }
            public string GetAppName()
            {
                return DAPFile.FullName.Substring(0, DAPFile.FullName.IndexOf(','));
            }
        }
    }
}
