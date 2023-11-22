using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using NuGet;
using DConfigOS_Core.Layer1.CoreServices;

namespace DConfigOS_Core.Layer2.CoreServices
{
    public class BaseAction : SABFramework.Core.SABAction
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            throw new NotImplementedException();
        }

        protected static IPackage GetInstalledPackage(CoreManager projectManager, string packageId)
        {
            var package = (from p in projectManager.GetInstalledPackages(packageId)
                           where p.Id == packageId
                           select p).ToList<IPackage>().FirstOrDefault<IPackage>();
            //if (package == null)
            //{
            //    throw new InvalidOperationException(string.Format("The package for package ID '{0}' is not installed in this website. Copy the package into the App_Data/packages folder.", packageId));
            //}
            return package;
        }

        protected CoreManager GetCoreManager(Controller controller)
        {
            string remoteSource = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "\\App_Data\\packages";
            return new CoreManager(remoteSource, controller.Request.MapPath("~/"));
        }
    }
}
