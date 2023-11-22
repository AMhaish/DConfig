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
    public class UDViewTypeFieldsAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsAPI viewFieldsAPI { get; set; }
        [Inject]
        public IViewTypesAPI viewTypesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<ViewField> ViewFields { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (ViewFields != null)
            {
                foreach (ViewField vf in ViewFields)
                {
                    if (vf.Id == 0)
                    {
                        var res = viewFieldsAPI.AddFieldToViewType(Id, vf);
                        if (res == ResultCodes.ObjectHasntFound)
                        {
                            return Json(new { result = "false", message = "View Type hasn't been found to be updated" });
                        }
                        if (res == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                    }
                    else
                    {
                        var res = viewFieldsAPI.UpdateViewField(vf);
                        if (res == ResultCodes.ObjectHasntFound)
                        {
                            return Json(new { result = "false", message = "Some fields hasn't been found, please do refresh" });
                        }
                        if (res == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                    }
                }
                var viewType = viewTypesAPI.GetViewType(Id);
                if (viewType != null)
                {
                    return Json(new { result = "true", obj = viewType });
                }
                else
                {
                    return Json(new { result = "false", message = "View Type hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No fields to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = viewFieldsAPI.RemoveFieldFromViewType(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Field hasn't been found to be deleted" });
            }
        }
    }
}
