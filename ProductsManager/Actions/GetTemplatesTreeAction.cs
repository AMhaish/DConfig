using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.ProductsManagerServices;
using CompetitiveAnalysis.Models;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetTemplatesTreeAction: SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = productsAPI.GetTemplates().Select(a => new TreeNodeModel()
            {
                id = a.Id.ToString(),
                obj = a,
                text = a.Name,
                type = ContentsTreeNodeType.Container
            });
            return Json(result);
        }
    }
}
