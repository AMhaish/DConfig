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
    public class GetProductsTemplatesTreeAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = productsAPI.GetTemplates();
            var propertiesGroups = propertiesAPI.GetPropertiesGroups();
            foreach (ProductsTemplate t in result)
            {
                var properties = t.Properties.ToList();
                t.Properties = new List<Property>();
                foreach (PropertiesGroup g in propertiesGroups)
                {
                    foreach (Property p in properties.Where(m => m.GroupId == g.Id).OrderBy(m => m.Priority))
                    {
                        t.Properties.Add(p);
                    }
                }
            }
            var finalResult = result.Select(a => new TreeNodeModel_HasChildren()
            {
                id =  a.Id.ToString(),
                obj = a,
                text = a.Name,
                type = ContentsTreeNodeType.Container,
                children= true
            });
            return Json(finalResult);
        }
    }
}
