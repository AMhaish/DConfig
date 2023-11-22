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
using DConfigOS_Core.Layer2.ActionsModels;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UScriptsOrderAction : UserActionsBase
    {
        [Inject]
        public IScriptsAPI scriptsAPI { get; set; }
        [Inject]
        public IScriptsBundlesAPI scriptsBundlesAPI { get; set; }

        [Required]
        public int BundleId { get; set; }
        [Required]
        public List<Script> Scripts { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = scriptsAPI.UpdateScriptsOrder(Scripts);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var scriptBundle = scriptsBundlesAPI.GetScriptBundle(BundleId);
                        if (scriptBundle != null)
                        {
                            TreeNodeModel treeObj = new TreeNodeModel()
                            {
                                id = "B" + scriptBundle.Id.ToString(),
                                obj = scriptBundle,
                                text = scriptBundle.Name,
                                type = ContentsTreeNodeType.Container,
                                children = scriptBundle.Scripts.Select(aa => new TreeNodeModel()
                                {
                                    id = aa.Id.ToString(),
                                    obj = aa,
                                    text = aa.Name,
                                    type = ContentsTreeNodeType.Item
                                }).ToList()
                            };
                            return Json(new { result = "true", obj = treeObj });
                        }
                        else
                        {
                            return Json(new { result = "false", message = "Style hasn't been found to be updated" });
                        }
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Scripts hasn't been found to be updated" });
                }
            }
            return Json(new { result = "false", message = "No data passed to be updated" });
        }
    }
}
