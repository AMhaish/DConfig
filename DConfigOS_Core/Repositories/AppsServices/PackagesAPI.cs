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
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public class PackagesAPI : IPackagesAPI
    {
        public virtual int InstallPackage(string path)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            AppDomain newDomain = AppDomain.CreateDomain("PackageSetup", AppDomain.CurrentDomain.Evidence, setup); //Create an instance of loader class in new appdomain 
            System.Runtime.Remoting.ObjectHandle obj = newDomain.CreateInstance(typeof(ComponentsInstaller).Assembly.FullName, typeof(ComponentsInstaller).FullName);
            ComponentsInstaller componentsInstaller = (ComponentsInstaller)obj.Unwrap();
            componentsInstaller.Initialize(path, SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath);
            string packageName = componentsInstaller.GetPackageName();
            var oldPackage = context.PublicViewsPackages.SingleOrDefault(m => m.Name == packageName);
            if (oldPackage != null)
            {
                UninstallPackage(packageName);
            }
            PublicViewsPackage package = componentsInstaller.InstallPackageComponents();
            AppDomain.Unload(newDomain);
            string distPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\bin\\publicPackages\\" + package.Name + ".dpp";
            File.Copy(path, distPath, true);
            package.Path = "/bin/publicPackages/" + package.Name + ".dpp";
            context.PublicViewsPackages.Add(package);
            context.SaveChanges();
            return ResultCodes.Succeed;
        }
        public virtual bool UninstallPackage(string appName)
        {
            return false;
            //DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            //var app = context.Apps.SingleOrDefault(m => m.Name == appName);
            //if (app != null && !app.BuiltInApp)
            //{
            //    string appModulePath = app.DllPath;
            //    var rootPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath;
            //    foreach (AppView v in app.AppViews)
            //    {
            //        var filePath = rootPath + v.Path.Replace('/', '\\');
            //        if (File.Exists(filePath))
            //            File.Delete(filePath);
            //    }
            //    foreach (AppWidget v in app.AppWidgets)
            //    {
            //        foreach (WidgetView wv in v.WidgetsViews)
            //        {
            //            var filePath = rootPath + wv.Path.Replace('/', '\\');
            //            if (File.Exists(filePath))
            //                File.Delete(filePath);
            //        }
            //        foreach (WidgetClientLogic wcl in v.WidgetClientLogics)
            //        {
            //            var filePath = rootPath + wcl.Path.Replace('/', '\\');
            //            if (File.Exists(filePath))
            //                File.Delete(filePath);
            //        }
            //    }
            //    foreach (AppExtention x in app.AppExtentions)
            //    {
            //        foreach (AppExtentionView xv in x.AppExtViews)
            //        {
            //            var filePath = rootPath + xv.Path.Replace('/', '\\');
            //            if (File.Exists(filePath))
            //                File.Delete(filePath);
            //        }
            //        foreach (AppExtentionClientLogic xcl in x.AppExtClientLogics)
            //        {
            //            var filePath = rootPath + xcl.Path.Replace('/', '\\');
            //            if (File.Exists(filePath))
            //                File.Delete(filePath);
            //        }
            //    }
            //    foreach (AppClientLogic v in app.AppClientLogics)
            //    {
            //        var filePath = rootPath + v.Path.Replace('/', '\\');
            //        if (File.Exists(filePath))
            //            File.Delete(filePath);
            //    }
            //    foreach (AppStyleSheet v in app.AppStyleSheets)
            //    {
            //        var filePath = rootPath + v.Path.Replace('/', '\\');
            //        if (File.Exists(filePath))
            //            File.Delete(filePath);
            //    }
            //    if (!String.IsNullOrEmpty(app.ConfigPath))
            //    {
            //        var filePath = rootPath + app.ConfigPath.Replace('/', '\\');
            //        if (File.Exists(filePath))
            //            File.Delete(filePath);
            //    }
            //    context.Apps.Remove(app);
            //    context.SaveChanges();
            //    SABFramework.Core.SABCoreEngine.Instance.UnloadModule(appName);
            //    var dapPath = rootPath + appModulePath.Replace('/', '\\');
            //    if (File.Exists(dapPath))
            //        File.Delete(dapPath);
            //}
            //return true;
        }
        class ComponentsInstaller : MarshalByRefObject
        {
            private Assembly DPPFile;
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
                DPPFile = System.Reflection.Assembly.LoadFrom(path);
            }

            private enum PublicPackageComponent { Template, Script, StyleSheet, Config }
            private void InstallPackageComponent(string packageName, string componentName, Stream componentStream, PublicPackageComponent componentType, PublicViewsPackage package, string rootPath)
            {
                switch (componentType)
                {
                    case PublicPackageComponent.Template:
                        if (package.PackageViewTemplates == null)
                            package.PackageViewTemplates = new List<ViewTemplate>();
                        package.PackageViewTemplates.Add(new ViewTemplate() { Name = componentName, Path = "/Views/PublicViews/Templates/" + packageName + "/" + componentName });
                        return;
                    case PublicPackageComponent.StyleSheet:
                        if (package.PackageStyleSheets == null)
                            package.PackageStyleSheets = new List<StyleSheet>();
                        package.PackageStyleSheets.Add(new StyleSheet() { Name = componentName, Path = "/Content/Public/" + packageName + "/" + componentName });
                        return;
                    case PublicPackageComponent.Script:
                        if (package.PackageScripts == null)
                            package.PackageScripts = new List<Script>();
                        package.PackageScripts.Add(new Script() { Name = componentName, Path = "/Scripts/Public/" + packageName + "/" + componentName });
                        return;
                    case PublicPackageComponent.Config:
                        //Read the configuration file and save it to the database
                        return;
                }
                StreamReader componentReader = new StreamReader(componentStream);
                StringBuilder distPath = new StringBuilder();
                distPath.Append(rootPath);
                switch (componentType)
                {
                    case PublicPackageComponent.Template:
                        distPath.Append("\\Views\\PublicViews\\Templates\\" + packageName);
                        break;
                    case PublicPackageComponent.Script:
                        distPath.Append("\\Scripts\\Public\\" + packageName);
                        break;
                    case PublicPackageComponent.StyleSheet:
                        distPath.Append("\\Content\\Public\\" + packageName);
                        break;
                }
                if (componentType != PublicPackageComponent.Config)
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
            }

            private PublicViewsPackage InstallPackageData(Stream configStream) 
            {
                PublicViewsPackage package;
                XDocument xmlFile = XDocument.Load(configStream);
                package = PublicViewsPackage.DeSerialize(xmlFile.ToString());
                return package;
            }

            public PublicViewsPackage InstallPackageComponents()
            {
                PublicViewsPackage package;
                if (DPPFile != null)
                {
                    var packageName=GetPackageName();
                    List<string> resourcesNames = DPPFile.GetManifestResourceNames().ToList();
                    string configResourceName = resourcesNames.Where(m => m.EndsWith(packageName + ".config")).FirstOrDefault();
                    var configShortName = configResourceName.Remove(0, packageName.Length + 1);
                    var configStream = DPPFile.GetManifestResourceStream(configResourceName);
                    package= InstallPackageData(configStream);
                    package.Name = packageName;
                    package.Version = DPPFile.FullName.Substring(DPPFile.FullName.IndexOf("Version=") + 8, 7);
                    package.InstalledDate = DateTime.Now;
                    foreach (string r in resourcesNames)
                    {
                        var resourceName = r.Remove(0, package.Name.Length + 1);
                        var resourceStream = DPPFile.GetManifestResourceStream(r);
                        if (resourceName.StartsWith("Templates."))
                        {
                            resourceName = resourceName.Remove(0, 10);
                            InstallPackageComponent(package.Name, resourceName, resourceStream, PublicPackageComponent.Template, package, rootPath);
                        }
                        else if (resourceName.StartsWith("Scripts."))
                        {
                            resourceName = resourceName.Remove(0, 8);
                            InstallPackageComponent(package.Name, resourceName, resourceStream, PublicPackageComponent.Script, package, rootPath);
                        }
                        else if (resourceName.StartsWith("Styles."))
                        {
                            resourceName = resourceName.Remove(0, 7);
                            InstallPackageComponent(package.Name, resourceName, resourceStream, PublicPackageComponent.StyleSheet, package, rootPath);
                        }
                        else if (resourceName.EndsWith(package.Name + ".config"))
                        {
                            continue;
                        }
                    }
                    return package;
                }
                return null;
            }

            public string GetPackageName()
            {
                return DPPFile.FullName.Substring(0, DPPFile.FullName.IndexOf(','));
            }
        }
    }
}
