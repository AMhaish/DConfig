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
    public class UProductPropertiesValues : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<ProductPropertyValue> Properties { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (Properties != null)
            {
                foreach (ProductPropertyValue p in Properties)
                {
                    var res = productsAPI.UpdateProductPropertyValue(Id, p.PropertyId, p.Value);
                    if (res==ResultCodes.ObjectHasntFound)
                    {
                        return Json(new { result = "false", message = "Product hasn't been found, please do refresh" });
                    }
                }
                Product pro=null;
                productsAPI.UpdateProductDate(Id,out pro);
                pro.Prices = pro.Prices.OrderByDescending(m => m.CreateDate).ToList();
                if (pro != null)
                {
                    return Json(new { result = "true", obj = pro });
                }
                else
                {
                    return Json(new { result = "false", message = "Product hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No fields values to be updated" });
            }
        }

    }
}
