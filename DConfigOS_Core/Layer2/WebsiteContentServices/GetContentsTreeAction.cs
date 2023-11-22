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
    public class GetContentsTreeAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        public int? Id { get; set; }

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
                        children = new List<TreeNodeModel>(),
                        plentyChildren = c.PlentyChildren
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
                    if (c.ParentId.HasValue)
                    {
                        if (c.Parent.ViewType != null)
                        {
                            c.PossibleViewTypes = c.Parent.ViewType.ChildrenTypes.Select(m => new ViewType() { Id=m.Id, Name=m.Name, TypeTemplates=m.TypeTemplates }).ToList();
                        }
                        else
                        {
                            c.PossibleViewTypes = viewTypesAPI.GetRootViewTypes().Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates }).ToList();
                        }
                    }
                    else
                    {
                        c.PossibleViewTypes = viewTypesAPI.GetRootViewTypes().Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates }).ToList();
                    }
                    if (c.ViewType != null)
                    {
                        c.PossibleChildViewTypes = c.ViewType.ChildrenTypes.Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates }).ToList();
                        c.PossibleChildViewTemplates = c.ViewType.TypeTemplates.Select(m => new ViewTemplate() { Id=m.Id, Name=m.Name }).ToList();
                    }
                    if (node.plentyChildren)
                    {
                        dest.Add(node);
                    }
                    else
                    {
                        dest.Add(node);
                        BuildContentsTreeNodes(node.children, c.ChildrenContents);
                    }
                    if (c.ContentInstances != null && c.ContentInstances.Count > 0)
                    {
                        foreach (ContentInstance cc in c.ContentInstances.Where(m => !m.StageId.HasValue))
                        {
                            var instanceNode = new TreeNodeModel()
                            {
                                id = "I" + cc.Id.ToString(),
                                obj = cc,
                                type = ContentsTreeNodeType.ContentVersion,
                                text = cc.Name
                            };
                            //if (c.ViewType != null)
                            //{
                                //cc.PossibleViewTemplates = c.ViewType.TypeTemplates.Select(m => new ViewTemplate() { Id = m.Id, Name = m.Name }).ToList();
                            //}
                            node.children.Add(instanceNode);
                        }
                    }
                }
            }
        }
    }
}
