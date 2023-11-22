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
    public class UStylesOrderAction : UserActionsBase
    {
        [Inject]
        public IStylesAPI stylesAPI { get; set; }
        [Inject]
        public IStylesBundlesAPI stylesBundlesAPI { get; set; }

        [Required]
        public int BundleId { get; set; }
        [Required]
        public List<StyleSheet> Styles { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = stylesAPI.UpdateStylesOrder(Styles);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var styleBundle = stylesBundlesAPI.GetStylesBundle(BundleId);
                        if (styleBundle != null)
                        {
                            TreeNodeModel treeObj = new TreeNodeModel()
                            {
                                id = "B" + styleBundle.Id.ToString(),
                                obj = styleBundle,
                                text = styleBundle.Name,
                                type = ContentsTreeNodeType.Container,
                                children = styleBundle.Styles.Select(aa => new TreeNodeModel()
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
                        return Json(new { result = "false", message = "Styles hasn't been found to be updated" });
                }
            }
            return Json(new { result = "false", message = "No data passed to be updated" });
        }
    }
}
