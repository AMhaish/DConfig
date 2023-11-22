using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.AppsServices
{
    public interface IPackagesAPI
    {
        int InstallPackage(string path);
        bool UninstallPackage(string appName);
    }
}
