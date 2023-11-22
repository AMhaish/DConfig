using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Models;
using Membership.MembershipServices;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using Ninject;

namespace Membership.Actions
{
    public class GetContentsTreeAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IContentsAPI ContentsAPI { get; set; }

        [Inject]
        public IContentPrivilegesAPI ContentPrivilegesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var contents = ContentsAPI.GetRootContents();
            var result = new List<TreeNodeModel>();
            var prvs = ContentPrivilegesAPI.GetPrivileges();
            BuildContentsTreeNodes(result, contents,prvs);
            return Json(result);
        }

        private void BuildContentsTreeNodes(List<TreeNodeModel> dest, IEnumerable<Content> src,List<ContentPrivilege> prvs)
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
                        addObj = prvs.Where(m => m.ContentId==c.Id).FirstOrDefault(),
                        children = new List<TreeNodeModel>(),
                        type = ContentsTreeNodeType.Item
                    };
                    dest.Add(node);
                    BuildContentsTreeNodes(node.children, c.ChildrenContents,prvs);
                }
            }
        }
    }
}
