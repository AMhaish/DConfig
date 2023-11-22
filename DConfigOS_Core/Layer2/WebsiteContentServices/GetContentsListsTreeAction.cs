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
    public class GetContentsListTreeAction : UserActionsBase
    {
        public int? Id { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<Content> contents;
            if (!Id.HasValue)
            {
                contents = contentsAPI.GetRootContents((UserBasedApps ? UserId : null));
            }
            else
            {
                contents = new List<Content>();
                contents.Add(contentsAPI.GetContent(Id.Value));
            }
            var result = new List<TreeNodeModel>();
            BuildContentsTreeNodes(result, contents);
            return Json(result);
        }

        private void BuildContentsTreeNodes(List<TreeNodeModel> dest, IEnumerable<Content> src)
        {
            if (src != null && src.Count() > 0)
            {
                foreach (Content c in src.OrderBy(m => m.Priority))
                {
                    var node = new TreeNodeModel()
                    {
                        id = c.Id.ToString(),
                        obj = c,
                        text = c.Name,
                        children = new List<TreeNodeModel>()
                    };
                    switch (c.ContentType)
                    {
                        case ContentTypes.Page:
                            node.type = ContentsTreeNodeType.Page;
                            break;
                        case ContentTypes.Partial:
                            node.type = ContentsTreeNodeType.Partial;
                            break;
                        case ContentTypes.Redirect:
                            node.type = ContentsTreeNodeType.Redirect;
                            break;
                    }
                    dest.Add(node);
                    BuildContentsTreeNodes(node.children, c.ChildrenContents);
                }
            }
        }
    }
}
