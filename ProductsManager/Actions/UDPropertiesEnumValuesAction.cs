using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class UDPropertiesEnumValuesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesEnumsAPI propertiesEnumsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<PropertyEnumValue> Values { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var result = propertiesEnumsAPI.UpdatePropertyEnumValues(Id, Values);
            var e = propertiesEnumsAPI.GetPropertiesEnum(Id);
            if (result==ResultCodes.Succeed)
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
            var result = propertiesEnumsAPI.DeletePropertyEnumValue(Id);
            if (result==ResultCodes.Succeed)
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
