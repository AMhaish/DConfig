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
    public class GetTemplatesTreeAction : UserActionsBase
    {
        [Inject]
        public ITemplatesAPI templatesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var templates = templatesAPI.GetRootTemplates( (UserBasedApps ? UserId : null));
            var result = new List<TreeNodeModel>();
            BuildTemplatesTreeNodes(result, templates);
            return Json(result);
        }

        private void BuildTemplatesTreeNodes(List<TreeNodeModel> dest, IEnumerable<ViewTemplate> src)
        {
            if (src != null && src.Count() > 0)
            {
                foreach (ViewTemplate c in src)
                {
                    var node = new TreeNodeModel()
                    {
                        id = c.Id.ToString(),
                        obj = c,
                        text = c.Name,
                        children = new List<TreeNodeModel>(),
                        type = (c.IsContainer ? ContentsTreeNodeType.Container : ContentsTreeNodeType.Item)
                    };
                    dest.Add(node);
                    BuildTemplatesTreeNodes(node.children, c.ChildrenTemplates);
                }
            }
        }
    }
}
