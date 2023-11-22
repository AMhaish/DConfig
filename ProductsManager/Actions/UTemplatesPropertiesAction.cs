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
    public class UTemplatesPropertiesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<ProductTemplatesPropertiesRelation> Properties { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            int res;
            if (Properties != null && Properties.Count > 0)
            {
                res = productsAPI.UpdateTemplateProperties(Id, Properties);
            }
            else
            {
                res = productsAPI.UpdateTemplateProperties(Id, null);
            }
            var template = productsAPI.GetTemplate(Id);
            switch (res)
            {
                case ResultCodes.Succeed:
                    return Json(new { result = "true", obj = template });
                case ResultCodes.ObjectHasntFound:
                    return Json(new { result = "false", message = "Template hasn't been found to be updated" });
                default:
                    return Json(new { result = "false" });
            }
        }

    }
}
