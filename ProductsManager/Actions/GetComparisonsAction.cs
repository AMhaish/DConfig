using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using CompetitiveAnalysis.ProductsManagerServices;
using CompetitiveAnalysis.Models;

namespace CompetitiveAnalysis.Actions
{
    public class GetComparisonsAction : SABFramework.Core.SABAction
    {
       private IComparisonAPI ComparisonsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = ComparisonsAPI.GetComparisons();
            foreach(Comparison c in result)
            {
                foreach(Product p in c.Products)
                {
                    p.Prices=p.Prices.OrderByDescending(m => m.PriceDate).ToList();
                }
            }
            return Json(result);
        }
    }
}
