using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetStagesbyRoleAction : UserActionsBase
    {
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            return Json(UserStages);
        }

    }
}
