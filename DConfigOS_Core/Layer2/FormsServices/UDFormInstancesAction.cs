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
    public class UDFormInstancesAction : UserActionsBase
    {
        [Inject]
        public IFormsInstancesAPI formsInstancesAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<FormFieldValue> FormFieldsValues { get; set; }
        public IFormFieldsAPI FormFieldsAPI { get; set; }
        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (FormFieldsValues != null)
            {
                foreach (FormFieldValue vf in FormFieldsValues)
                {
                    FormFieldsAPI.UpdateFormFieldValue(vf.FormInstanceId, vf.FieldId, vf.Value);
                }
                var form = formsInstancesAPI.GetFormInstance(Id);
                if (form != null)
                {
                    return Json(new { result = "true", obj = form });
                }
                else
                {
                    return Json(new { result = "false", message = "Posted form hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No fields to be updated" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            var result = formsInstancesAPI.DeleteFormInstance(Id);
            if (result == ResultCodes.Succeed)
            {
                return Json(new { result = "true"});
            }
            else
            {
                return Json(new { result = "false", message = "Posted form hasn't been found to be deleted" });
            }
        }
    }
}
