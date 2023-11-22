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
using System.IO;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class CUDProductsAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public int TemplateId { get; set; }
        [Required]
        public string Name { get; set; }
        public string BrandFactoryType { get; set; }
        public int? CompanyId { get; set; }
        public string Notes { get; set; }
        public string Code { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var p = new Product()
                {
                    Id = Id,
                    Name = Name,
                    TemplateId = TemplateId,
                    BrandFactoryType = BrandFactoryType,
                    CompanyId = CompanyId,
                    Notes = Notes,
                    Code=Code,
                };
                var result = productsAPI.CreateProduct(p);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        //Initizlize resources folder for product
                        Directory.CreateDirectory(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources/Produtcs" + p.Id);
                        return Json(new { result = "true", obj = p });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Product with the same name is already exists" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id, Name and product Id are required to add a product" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var p = new Product()
                {
                    Id = Id,
                    Name = Name,
                    TemplateId = TemplateId,
                    BrandFactoryType = BrandFactoryType,
                    CompanyId = CompanyId,
                    Notes=Notes,
                    Code=Code
                };
                var result = productsAPI.UpdateProduct(p);
                var pp = productsAPI.GetProduct(p.Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = pp });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Product hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "There is already product with the same name" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id, Name and product Id are required to update the product" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (Id != 0)
            {
                var result = productsAPI.DeleteProduct(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        Directory.Delete(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/Resources/Produtcs" + Id,true);
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Product hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id is required to delete the product" });
            }
        }
    }
}
