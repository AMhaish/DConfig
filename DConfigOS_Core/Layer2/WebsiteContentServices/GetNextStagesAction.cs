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
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using System.IO;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetNextStagesAction : UserActionsBase
    {


        public int? Id { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (Id.HasValue)
                return Json(stagesAPI.GetNextStages(Id.Value));
            else
                return Json(stagesAPI.GetNextStages(UserStages.Select(m => m.Id).ToList()));
        }
    }
}
