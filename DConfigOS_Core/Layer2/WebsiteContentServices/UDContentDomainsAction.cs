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
using DConfigOS_Core.Providers.HttpContextProviders;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UDContentDomainsAction : UserActionsBase
    {
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public string Domain { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(Domain))
            {
                var result = contentsAPI.AddDomainToContent(Id, Domain.ToLower());
                DConfigRequestContext.InitializeDomains();
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var obj = contentsAPI.GetContent(Id);
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                    case ResultCodes.ObjectNameAlreadyUsed:
                        return Json(new { result = "false", message = "Can't add this domain because it is used before in this content or another content" });
                    default:
                        return Json(new { result = "false", message = "Unknown error" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Domain parameter is missing" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(Domain))
            {
                var result = contentsAPI.RemoveDomainFromContent(Id, Domain.ToLower());
                DConfigRequestContext.InitializeDomains();
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var obj = contentsAPI.GetContent(Id);
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false", message = "Unknown error" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Domain parameter is missing" });
            }
        }
    }
}
