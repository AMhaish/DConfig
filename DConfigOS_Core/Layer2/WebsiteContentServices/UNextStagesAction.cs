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
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UNextStagesAction : UserActionsBase
    {
        [Inject]
        public IStagesAPI stagesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<int> ids { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var res = stagesAPI.UpdateNextStages(Id, ids);
            if (res == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Stage hasn't been found to be updated" });
            }
        }

    }
}
