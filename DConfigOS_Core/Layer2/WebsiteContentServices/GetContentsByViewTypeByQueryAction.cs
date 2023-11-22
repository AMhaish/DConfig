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
    public class GetContentsByViewTypeByQueryAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public int Id { get; set; }
        public string keyword { get; set; }
        public int limit { get; set; }
        public int skip { get; set; }
        public string sortField { get; set; }
        public string sortOrder { get; set; }
        public int? Days { get; set; }
        public int? ParentId { get; set; }
        public string KeywordField { get; set; } = null;

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            // This has been set hardcoded with talk with Ahmad
            QueryResults<Content> result;
            if (Days.HasValue)
                result = contentsAPI.GetContentsByViewTypeByQuery(Id, limit, skip, keyword, null, ParentId, KeywordField, sortField, sortOrder);
            else
                result = contentsAPI.GetContentsByViewTypeByQuery(Id, limit, skip, keyword, 5, ParentId, KeywordField, sortField, sortOrder);
            foreach (var c in result.Items)
            {
                c.ContentInstances = contentsAPI.GetContentInstances(c.Id);
            }
            return Json(result);
        }
    }
}
