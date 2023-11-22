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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DConfigOS_Core.Layer2.WebsiteContentServices
{
    public class UContentFieldsValuesAction : UserActionsBase
    {
        [Inject]
        public IViewFieldsAPI viewFieldsAPI { get; set; }
        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        [Required]
        public int Id { get; set; }
        public List<ViewFieldValue> FieldsValues { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (FieldsValues != null)
            {
                Dictionary<string, string> dataMap;
                ContentInstance contentInstance = contentsAPI.GetContentInstance(Id);
                if (!String.IsNullOrEmpty(contentInstance.Data))
                {
                    dataMap = contentInstance.DataDic;
                }
                else
                {
                    dataMap = new Dictionary<string, string>();
                }
                foreach (ViewFieldValue f in FieldsValues)
                {
                    var res = viewFieldsAPI.UpdateViewFieldValue(Id, f.FieldId, f.Value);
                    var field = viewFieldsAPI.GetViewTypeField(f.FieldId);
                    if (dataMap.ContainsKey(field.Name))
                    {
                        dataMap[field.Name] = f.Value;
                    }
                    else
                    {
                        dataMap.Add(field.Name, f.Value);
                    }
                    if (res == ResultCodes.ObjectHasntFound)
                    {
                        return Json(new { result = "false", message = "Content hasn't been found, please do refresh" });
                    }
                }
                contentInstance.DataDic = dataMap;
                contentsAPI.UpdateContentInstance(contentInstance);
                if (contentInstance != null)
                {
                    return Json(new { result = "true", obj = contentInstance });
                }
                else
                {
                    return Json(new { result = "false", message = "Content hasn't been found to be updated" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No fields values to be updated" });
            }
        }

    }
}
