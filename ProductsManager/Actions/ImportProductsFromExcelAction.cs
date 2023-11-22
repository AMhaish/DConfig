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
    public class ImportProductsFromExcelAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IProductsAPI productsAPI { get; set; }

        public int TemplateId { get; set; }
        public string ExcelPath { get; set; }
        public int? CompanyId { get; set; }
        public string BrandFactoryType { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            ProductsAPI.ImportingReport report;
            var result = productsAPI.ImportDataFromExcel(TemplateId,ExcelPath,CompanyId, BrandFactoryType, out report);
            return Json(report);
        }
    }
}
