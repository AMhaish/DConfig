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
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.FormsServices
{
    public class FormSubmitFieldValueAction : UserActionsBase
    {
        [Inject]
        public IFormFieldsAPI formFieldsAPI { get; set; }
        [Inject]
        public IFoldersAPI foldersAPI { get; set; }

        [Required]
        public int FormId { get; set; }
        [Required]
        public int FieldId { get; set; }
        [Required]
        public string Value { get; set; }
        public bool IsFile { get; set; }
        public string Path { get; set; }

        

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            string savingPath=null;
            int result;
            if (controller.ModelState.IsValid)
            {
                if (IsFile)
                {
                    if (controller.HttpContext.Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase files = controller.HttpContext.Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            result = await foldersAPI.CreateFile(Path, file, out savingPath);
                            switch (result)
                            {
                                case ResultCodes.Succeed:
                                    continue;
                                case ResultCodes.ObjectEmpty:
                                    return Json(new { result = "false", message = "Some files is empty so it hasn't been uploaded" });
                                case ResultCodes.ObjectSavedWithAnotherName:
                                    return Json(new { result = "true", message = "Some files has been saved with another name because another file with the same name is already exists", obj= savingPath });
                                default:
                                    return Json(new { result = "false", message = "Unknown error happened when saving some files" });
                            }
                        }
                        return Json(new { result = "true", obj = savingPath });
                    }
                    return Json(new { result = "false", message = "No uploaded data have been provieded" });
                }
                var fieldUpdateResult = formFieldsAPI.UpdateFormFieldValue(FormId, FieldId, (IsFile ? Path : Value));
                switch (fieldUpdateResult)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Field hasn't been found to be updated" });
                }
            }
            return Json(new { result = "false", message = "Path is required" });
        }
    }
}
