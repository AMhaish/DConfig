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
    public class UContentFieldValueAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsAPI viewFieldsAPI { get; set; }

        [Required]
        public int FieldId { get; set; }
        public int ContentId { get; set; }
        public string Value { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var res = viewFieldsAPI.UpdateViewFieldValue(ContentId, FieldId, Value);
            if (res == ResultCodes.ObjectHasntFound)
            {
                return Json(new { result = "false", message = "Content hasn't been found." });
            }
            return Json(new { result = "true" });
        }

    }
}
