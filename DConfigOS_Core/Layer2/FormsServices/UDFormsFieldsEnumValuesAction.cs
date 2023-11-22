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

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class UDFormsFieldsEnumValuesAction : UserActionsBase
    {
        [Inject]
        public IFormsFieldsEnumsAPI formsFieldsEnumsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<FormsFieldsEnumValue> Values { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            int result;
            if(Values!=null && Values.Count>0)
            {
                result = formsFieldsEnumsAPI.UpdateFormsFieldsEnumValues(Id, Values.Select(m => m.Value).ToList());
            }
            else
            {
                result = formsFieldsEnumsAPI.UpdateFormsFieldsEnumValues(Id, null);
            }
            var e= formsFieldsEnumsAPI.GetFormsFieldsEnum(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true", obj = e });
            }
            else
            {
                return Json(new { result = "false", message = "Predefined List hasn't been found to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = formsFieldsEnumsAPI.DeleteFormsFieldsEnumValue(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "Value hasn't been found to be deleted" });
            }
        }
    }
}
