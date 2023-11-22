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
    public class GetTemplateAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public int Id { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = productsAPI.GetTemplate(Id);
            var propertiesGroups = propertiesAPI.GetPropertiesGroups();
            var properties = result.Properties.ToList();
            result.Properties=new List<Property>();
            foreach(PropertiesGroup g in propertiesGroups)
            {
                foreach (Property p in properties.Where(m => m.GroupId==g.Id).OrderBy(m => m.Priority))
                {
                    result.Properties.Add(p);
                }
            }
            return Json(result);
        }
    }
}
