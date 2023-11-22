using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Layer2.ActionsModels;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class GetTemplateProductsByPatternAction : AppBaseAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public int Id { get; set; }
        public string Pattern { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            List<Product> result;
            List<string> BrandFactoryTypes = new List<string>();
            //foreach (AdvancedPrivilege ap in UserAdvancedPrivileges)
            //{
                //BrandFactoryTypes.AddRange(ap.RelatedBrandFactoyTypes.Split(','));
            //}
            switch (Pattern)
            {
                case "*":
                    result = productsAPI.GetTemplateProducts(Id).ToList(); //.Where(m => BrandFactoryTypes.Contains(m.BrandFactoryType))
                    break;
                default:
                    result = productsAPI.GetTemplateProductsByPattern(Id,Pattern).ToList(); //Where(m => BrandFactoryTypes.Contains(m.BrandFactoryType))
                    break;

            }
            foreach (Product p in result)
            {
                p.Prices = p.Prices.OrderByDescending(m => m.PriceDate).ToList();
            }
            return Json(result);
        }
    }
}
