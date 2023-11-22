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
using DConfigOS_Core.Providers.HttpContextProviders;
using Ninject;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class RenderFormPrintVersionAction : FormBaseAction
    {
        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var formInstance = formsAPI.GetFormInstance(Id);
            if (formInstance.Form.PrintTemplateId.HasValue)
            {
                var model = new DConfigModel();
                model.FieldsDictionaryOnId = formInstance.FieldsValues.ToDictionary(m => m.FieldId, m => m.Value);
                foreach(FormInstance child in formInstance.ChildrenInstances)
                {
                    foreach(FormFieldValue fv in child.FieldsValues)
                    {
                        model.FieldsDictionaryOnId.Add(fv.FieldId, fv.Value);
                    }
                }
                return View(formInstance.Form.PrintTemplate.Path, model);
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}
