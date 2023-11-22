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
    public class UProductTags : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<string> Tags { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (Tags != null)
            {
                var result = productsAPI.UpdateProductTags(Id, Tags);
                if (result == ResultCodes.Succeed)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "Products hasn't been found to be updated with new tags." });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Error, No tags has been post to the system." });
            }
        }

    }
}
