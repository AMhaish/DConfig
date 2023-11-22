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
    public class ExportTemplateExcelAction : UserActionsBase
    {
        public int ContentId { get; set; }
        public int ViewTypeId { get; set; }

        [Inject]
        public IContentsAPI contentsAPI { get; set; }
      
        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            var result = contentsAPI.ExportTemplateAsExcel(ContentId, ViewTypeId);
            return Download(result,"Template.xlsx");
        }
    }
}
