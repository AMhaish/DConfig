using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.AppsServices;
using Ninject;
using Newtonsoft.Json;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class AppsAction: UserActionsBase
    {
        [Inject]
        [JsonIgnore]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (UserIsAdministrator)
                return Json(appAPIRepository.GetInstalledApps());
            else
                return Json(appAPIRepository.GetInstalledApps(User.Roles.Select(m => m.RoleId).ToList()));
        }
    }
}
