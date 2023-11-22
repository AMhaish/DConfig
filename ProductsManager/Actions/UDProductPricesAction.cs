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
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class UDProductPricesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<Price> Prices { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (Prices != null)
            {
                foreach (Price p in Prices)
                {
                    if (p.Id == 0)
                    {
                        var res = productsAPI.AddPriceToProduct(Id, p);
                        if (res!=ResultCodes.Succeed)
                        {
                            return Json(new { result = "false", message = "Product hasn't been found to be updated" });
                        }
                    }
                    else
                    {
                        var res = productsAPI.UpdateProductPrice(p);
                        if (res != ResultCodes.Succeed)
                        {
                            return Json(new { result = "false", message = "Price hasn't been found to be updated" });
                        }
                    }
                }
                var product = productsAPI.GetProduct(Id);
                product.Prices = product.Prices.OrderByDescending(m => m.CreateDate).ToList();
                if (product != null)
                {
                    return Json(new { result = "true", obj = product });
                }
                else
                {
                    return Json(new { result = "false", message = "Product hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No prices to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = productsAPI.RemovePriceFromProduct(Id);
            if (result==ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Price hasn't been found to be deleted" });
            }
        }
    }
}
