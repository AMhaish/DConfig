using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Models;
using System.Linq;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    class GetQuickContentList : UserActionsBase
    {
        public int Id { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<Content> contents;
            List<ViewTemplate> templates = new List<ViewTemplate>(); 
            contents = contentsAPI.GetQuickContents((UserBasedApps ? UserId : null));
            foreach (Content c in contents)
            {
                if (c.ViewType != null)
                {
                    c.PossibleChildViewTypes = c.ViewType.ChildrenTypes.Select(m => new ViewType() { Id = m.Id, Name = m.Name, TypeTemplates = m.TypeTemplates, ViewFields=m.ViewFields }).ToList();
                    foreach(var cc in c.PossibleChildViewTypes)
                    {
                        templates.AddRange(cc.TypeTemplates.Select(m => new ViewTemplate() { Id = m.Id, Name = m.Name }));
                    }
                    c.PossibleChildViewTemplates = templates;
                }
            }
            return Json(contents);
        }

    }
    
   
}
