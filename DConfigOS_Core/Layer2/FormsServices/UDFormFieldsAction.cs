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
    public class UDFormFieldsAction : UserActionsBase
    {
        [Inject]
        public IFormFieldsAPI formFieldsAPI { get; set; }
        [Inject]
        public IFormsAPI formsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<FormsField> FormFields { get; set; }
        
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (FormFields != null)
            {
                foreach (FormsField vf in FormFields)
                {
                    if (vf.Id == 0)
                    {
                        var res = formFieldsAPI.AddFieldToForm(Id, vf);
                        if (res == ResultCodes.ObjectHasntFound)
                        {
                            return Json(new { result = "false", message = "Form hasn't been found to be updated" });
                        }
                        else if (res == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                        else if(res == ResultCodes.ObjectNotAllowedToBeUpdated)
                        {
                            return Json(new { result = "false", message = "Can't add fileds cause the form is linked with an app" });
                        }
                    }
                    else
                    {
                        var res = formFieldsAPI.UpdateFormField(vf);
                        if (res == ResultCodes.ObjectHasntFound)
                        {
                            return Json(new { result = "false", message = "Some fields hasn't been found, please do refresh" });
                        }
                        else if (res == ResultCodes.ObjectNameAlreadyUsed)
                        {
                            return Json(new { result = "false", message = "Field with name " + vf.Name + " already exists" });
                        }
                        else if (res == ResultCodes.ObjectNotAllowedToBeUpdated)
                        {
                            return Json(new { result = "false", message = "Can't update fileds cause the form is linked with an app" });
                        }
                    }
                }
                var form = formsAPI.GetForm(Id, null);
                if (form != null)
                {
                    return Json(new { result = "true", obj = form });
                }
                else
                {
                    return Json(new { result = "false", message = "Form hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No fields to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = formFieldsAPI.RemoveFieldFromForm(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else if(result == ResultCodes.ObjectNotAllowedToBeDeleted)
            {
                return Json(new { result = "false", message = "Form fileds can't be deleted cause the form is linked with an app" });
            }
            else
            {
                return Json(new { result = "false", message = "Field hasn't been found to be deleted" });
            }
        }
    }
}
