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
    public class UDViewFieldsEnumValuesAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsEnumsAPI viewFieldsEnumsAPI { get; set; }
        
        //[Required]
        public int Id { get; set; }
        public List<ViewFieldsEnumValue> Values { get; set; }
       
        public int[] Ids { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            int result;
            if (Values != null && Values.Count > 0)
            {
                result = viewFieldsEnumsAPI.UpdateViewFieldsEnumValues(Id, Values);
            }
            else
            {
                result = viewFieldsEnumsAPI.UpdateViewFieldsEnumValues(Id, null);
            }
            var e = viewFieldsEnumsAPI.GetViewFieldsEnum(Id);
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
            if (Ids != null && Ids.Count() > 0)
            {
                int result = 0;
                foreach (int id in Ids)
                {
                    result *= viewFieldsEnumsAPI.DeleteFormsFieldsEnumValue(id);
                }
                if (result == ResultCodes.Succeed) //which is 1
                    return Json(new { result = "true" });
                else
                    return Json(new { result = "false", message = "Some Values haven't been found to be deleted" });
            }
            else
            {
                var result = viewFieldsEnumsAPI.DeleteFormsFieldsEnumValue(Id);
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
}
