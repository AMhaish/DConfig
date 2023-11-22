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
    public class GetTemplateProductsByFiltersAction : AppBaseAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public int Id { get; set; }
        public List<string> BrandTypes { get; set; }
        public List<string> Tags { get; set; }
        public List<ProductsAPI.Filter> AndFilters { get; set; }
        public List<ProductsAPI.Filter> OrFilters { get; set; }
        public DateTime?[] CreateDateRange { get; set; }
        public DateTime?[] UpdateDateRange { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            List<string> BrandFactoryTypes = new List<string>();
            //foreach (AdvancedPrivilege ap in UserAdvancedPrivileges)
            //{
            //    BrandFactoryTypes.AddRange(ap.RelatedBrandFactoyTypes.Split(','));
            //}
            var result = productsAPI.GetTemplateProductsByFilters(Id, BrandTypes, Tags, AndFilters, OrFilters, CreateDateRange, UpdateDateRange).ToList();//.Where(m => BrandFactoryTypes.Contains(m.BrandFactoryType))
            foreach (Product p in result)
            {
                p.Prices = p.Prices.OrderByDescending(m => m.PriceDate).ToList();
            }
            return Json(result);
        }
    }
}
