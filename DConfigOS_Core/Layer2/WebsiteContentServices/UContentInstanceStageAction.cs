using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UContentInstanceStageAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        [Required]
        public int ContentId { get; set; }
        public string Comments { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && Id >0 && ContentId > 0)
            {
                ContentInstance resultObj;
                var result = contentsAPI.StagingContentInstance(ContentId, Id, Comments, out resultObj,UserId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = resultObj });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                }
            }
            return null;
        }
    }
}
