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
    public class GetProductsByIdsAction : AppBaseAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public List<int> Ids { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            List<string> BrandFactoryTypes = new List<string>();
            foreach(AdvancedPrivilege ap in UserAdvancedPrivileges)
            {
                BrandFactoryTypes.AddRange(ap.RelatedBrandFactoyTypes.Split(','));
            }
            var result = productsAPI.GetProductsByIds(Ids).Where(m => BrandFactoryTypes.Contains(m.BrandFactoryType)).ToList();
            foreach (Product p in result)
            {
                p.Prices = p.Prices.OrderByDescending(m => m.PriceDate).ToList();
            }
            return Json(result);
        }
    }
}
