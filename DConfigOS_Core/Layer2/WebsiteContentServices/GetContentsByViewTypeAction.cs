using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetContentsByViewTypeAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public int Id { get; set; }
        public int? Days { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            // This has been set hardcoded with talk with Ahmad
            List<Content> result;
            if (Days.HasValue)
                result = contentsAPI.GetContnetsByViewType(Id);
            else
                result = contentsAPI.GetContnetsByViewType(Id, 5);
            foreach (var c in result)
            {
                c.ContentInstances = contentsAPI.GetContentInstances(c.Id);
            }
            return Json(result);
        }
    }
}
