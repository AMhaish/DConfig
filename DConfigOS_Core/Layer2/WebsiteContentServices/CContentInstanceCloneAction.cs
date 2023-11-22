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
    public class CContentInstanceCloneAction : UserActionsBase
    {
        [Required]
        public int Id { get; set; }
        public string Suffix { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                ContentInstance contentInstance;
                var result = contentsAPI.CloneContentInstance(Id,out contentInstance, Suffix, (UserBasedApps?UserId:null));
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var viewType = contentsAPI.GetContent(contentInstance.ContentId).ViewType;
                        //if (viewType != null)
                            //contentInstance.PossibleViewTemplates = viewType.TypeTemplates.ToList();
                        return Json(new { result = "true", obj = contentInstance });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content Instance hasn't been found to be cloned." });
                }
            }
            return null;
        }
    }
}
