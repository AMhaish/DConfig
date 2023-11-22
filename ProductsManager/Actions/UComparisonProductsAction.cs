using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Repositories.Utilities;

namespace CompetitiveAnalysis.Actions
{
    public class UComparisonProductsAction : SABFramework.Core.SABAction
    {
        [Required]
        public int Id { get; set; }
        public List<Product> Products { get; set; }
        private IComparisonAPI ComparisonsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            int res;
            if (Products != null && Products.Count > 0)
            {
                res = ComparisonsAPI.UpdateComparisonProducts(Id, Products.Select(m => m.Id).ToList());
            }
            else
            {
                res = ComparisonsAPI.UpdateComparisonProducts(Id, null);
            }
            var comparison = ComparisonsAPI.GetComparison(Id);
            switch (res)
            {
                case ResultCodes.Succeed:
                    return Json(new { result = "true", obj = comparison });
                case ResultCodes.ObjectHasntFound:
                    return Json(new { result = "false", message = "Comparison hasn't been found to be updated" });
                default:
                    return Json(new { result = "false" });
            }
        }

    }
}
