using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public interface IAppAPIRepository  : IDisposable
    {
        App GetAppInfo(string appName);
        List<App> GetInstalledApps(List<string> roles = null);
        int UpdateAppRoles(string appName, List<string> roles);
        List<AppClientLogic> GetAppClientLogics(string appName);
        List<AppStyleSheet> GetAppStyles(string appName);
        int CheckAppNewUpdates(string appName, out string version);
        int GetAppVersionMetaData(string appName, string version, out FileMetaData metaData);
        int DownloadAppVersionFile(string appName, string version, Guid deviceKey, string userName, string password, out string dapFilePath);
        int InstallApp(string path);
        bool UninstallApp(string appName);


    }
}
