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
    public class GetScriptsTreeAction : UserActionsBase
    {
        [Inject]
        public IScriptsBundlesAPI scriptsBundlesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var scripts = scriptsBundlesAPI.GetScriptsBundles((UserBasedApps ? UserId : null)).Select(a => new TreeNodeModel()
            {
                id = "B" + a.Id.ToString(),
                obj = a,
                text = a.Name,
                type = ContentsTreeNodeType.Container,
                children = a.Scripts.Select(aa => new TreeNodeModel()
                {
                    id = aa.Id.ToString(),
                    obj = aa,
                    text = aa.Name,
                    type = ContentsTreeNodeType.Item
                }).ToList()
            });
            return Json(scripts);
        }
    }
}
