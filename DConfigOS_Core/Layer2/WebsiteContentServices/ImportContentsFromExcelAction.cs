using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Layer2.ActionsModels;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class ImportContentsFromExcelAction : UserActionsBase
    {
        public int ContentId { get; set; }
        public string ExcelPath { get; set; }
        public int ViewTypeId { get; set; }


        [Inject]
        public IContentsAPI contentsAPI { get; set; }
      
        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            ContentsAPI.ImportingReport report;
            var result = contentsAPI.ImportDataFromExcel(ContentId, ExcelPath, ViewTypeId, out report);
            return Json(report);
        }
    }
}
