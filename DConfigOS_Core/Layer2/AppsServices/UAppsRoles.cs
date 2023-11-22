using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.AppsServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.AppsServices
{
    public class UAppsRoles : UserActionsBase
    {
        [Required]
        public string Id { get; set; }
        public List<string> RolesIds { get; set; }

        [Inject]
        public IAppAPIRepository appAPIRepository { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var res = appAPIRepository.UpdateAppRoles(Id, RolesIds);
            if (res == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "App hasn't been found to be updated" });
            }
        }

    }
}
