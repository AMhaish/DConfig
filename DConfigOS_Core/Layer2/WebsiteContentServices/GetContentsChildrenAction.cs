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
    public class GetContentsChildrenAction : UserActionsBase
    {
        public string keyword { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        public int limit { get; set; }
        public int skip { get; set; }
        public int Id { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<Content> result=null;
            if (UserIsAdministrator || UserStages.Count <= 0)
            {
                result = contentsAPI.GetContentChildren(Id, limit, skip, keyword);

            }
            else
            {
                result = contentsAPI.GetContentChildrenBasedOnStages(Id, limit, skip, UserStages.Select(m => m.Id).ToList(), null);
            }
            foreach (Content c in result)
            {
                if (c.ParentId.HasValue)
                {
                    if (c.Parent.ViewType != null)
                    {
                        c.PossibleViewTypes = c.Parent.ViewType.ChildrenTypes.Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates, ViewFields = m.ViewFields }).ToList();
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
                    c.PossibleChildViewTypes = c.ViewType.ChildrenTypes.Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates, ViewFields = m.ViewFields }).ToList();
                    c.PossibleChildViewTemplates = c.ViewType.TypeTemplates.Select(m => new ViewTemplate() { Id=m.Id, Name=m.Name }).ToList();
                }
            }
            return Json(result);
        }
    }
}
