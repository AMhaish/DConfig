﻿using System.Threading.Tasks;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using System.Linq;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetContentInstancesForPrevVersionsAction : UserActionsBase
    {
        public int Id { get; set; }

        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (UserStages != null && UserStages.Count > 0)
            {
                return Json(contentsAPI.GetContentInstancesPrevVersionsBasedOnStages(Id, UserStages.Select(m => m.Id).ToList()));
            }
            else
            {
                return Json(null);
            }
        }
    }
}
