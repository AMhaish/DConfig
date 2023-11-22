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
    public class GetTemplatesAction : AppBaseAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        public bool? All { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<ProductsTemplate> result;
            result = productsAPI.GetTemplates();
            /*if (All.HasValue && All.Value)
            {
                result = productsAPI.GetTemplates();
            }
            else
            {
                result = new List<ProductsTemplate>();
                if (UserAdvancedPrivileges != null)
                {
                    foreach (AdvancedPrivilege ap in UserAdvancedPrivileges)
                    {
                        result.AddRange(ap.RelatedProdutTemplates);
                    }
                }
            }*/
            var propertiesGroups = propertiesAPI.GetPropertiesGroups();
            foreach (ProductsTemplate t in result)
            {
                var properties = productsAPI.GetTemplateProperties(t.Id);
                t.Properties = new List<Property>();
                foreach (PropertiesGroup g in propertiesGroups)
                {
                    foreach (Property p in properties.Where(m => m.GroupId == g.Id).OrderBy(m => m.Priority))
                    {
                        t.Properties.Add(p);
                    }
                }
            }
            return Json(result);
        }
    }
}
