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
using System.IO;
using Ninject;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class GetViewTypesChildrenAction : UserActionsBase
    {
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        [Required]
        public int Id { get; set; }

        public int? templateContextId { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            if (templateContextId.HasValue)
            {
                var result = viewTypesAPI.GetViewTypesChildrenByContextId(Id, templateContextId.Value);
                return Json(result);
            }
            else
            {
                var children = viewTypesAPI.GetViewTypeChildren(Id);
                return Json(children);
            }
        }
    }
}
