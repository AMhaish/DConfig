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
    public class CUViewFieldsEnumValueAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsEnumsAPI viewFieldsEnumsAPI { get; set; }
        public int Id { get; set; }
        public int EnumId { get; set; }
        public int? SubEnumId { get; set; }
        public string Value { get; set; }
        public string LangValueJson { get; set; }
        public int? Priority { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            var result = viewFieldsEnumsAPI.CreateViewFieldsEnumValue(EnumId, Value, SubEnumId, (UserBasedApps ? UserId : null), LangValueJson, Priority);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Predefined List hasn't been found to be updated" });
            }
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var result = viewFieldsEnumsAPI.UpdateViewFieldsEnumValue(EnumId, Id, Value, SubEnumId, (UserBasedApps ? UserId : null), LangValueJson, Priority);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Predefined List hasn't been found to be updated" });
            }
        }
    }
}
