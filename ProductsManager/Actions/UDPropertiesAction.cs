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
    public class UDPropertiesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IPropertiesAPI propertiesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<Property> Properties { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (Properties != null)
            {
                foreach (Property p in Properties)
                {
                    if (p.Id == 0)
                    {
                        var res = propertiesAPI.AddPropertyToGroup(Id, p);
                        if (res != ResultCodes.Succeed)
                        {
                            return Json(new { result = "false", message = "Group hasn't been found to be updated or the name of some properties are duplicated" });
                        }
                    }
                    else
                    {
                        var res = propertiesAPI.UpdateGroupProperty(p);
                        if (res != ResultCodes.Succeed)
                        {
                            return Json(new { result = "false", message = "Some properties hasn't been found, or there are duplicated in name." });
                        }
                    }
                }
                var group = propertiesAPI.GetPropertiesGroup(Id);
                if (group != null)
                {
                    return Json(new { result = "true", obj = group });
                }
                else
                {
                    return Json(new { result = "false", message = "Group hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No properties to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = propertiesAPI.RemovePropertyFromGroup(Id);
            if (result==ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Property hasn't been found to be deleted" });
            }
        }
    }
}
