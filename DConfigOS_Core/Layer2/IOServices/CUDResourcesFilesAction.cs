﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.IOServices
{
    public class CUDResourcesFilesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IFoldersAPI foldersAPI { get; set; }

        [Required]
        public string Path { get; set; }
        public string NewPath { get; set; }

        public bool? DconfigReq { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            string savingPath = null;
            if (controller.ModelState.IsValid)
            {
                int result = 0;
                if (controller.HttpContext.Request.Files.Count > 0)
                {
                    HttpFileCollectionBase files = controller.HttpContext.Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file != null && file.ContentLength > 0)
                        {
                            result = await foldersAPI.CreateFile(Path, file, out savingPath, DconfigReq);
                            switch (result)
                            {
                                case ResultCodes.Succeed:
                                    continue;
                                case ResultCodes.ObjectEmpty:
                                    return Json(new { result = "false", message = "Some files is empty so it hasn't been uploaded" });
                                case ResultCodes.ObjectSavedWithAnotherName:
                                    return Json(new { result = "true", message = "Some files has been saved with another name because another file with the same name is already exists", obj = savingPath });
                                default:
                                    return Json(new { result = "false", message = "Unknown error happened when saving some files" });
                            }
                        }
                        else
                        {
                            return Json(new { result = "false", message = "Empty file." });
                        }
                    }
                    return Json(new { result = "true", obj = savingPath });
                }
                return Json(new { result = "false", message = "No uploaded data have been provieded" });
            }
            return Json(new { result = "false", message = "Path is required" });
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && !String.IsNullOrEmpty(NewPath))
            {
                var result = await foldersAPI.MoveFile(Path, NewPath);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "File hasn't been found to be updated" });
                    default:
                        return Json(new { result = "false", message = "Unknown error" });
                }
            }
            return Json(new { result = "false", message = "Path and New Path are required" });
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = await foldersAPI.DeleteFile(Path);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "File hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false", message = "Unknown error" });
                }
            }
            return Json(new { result = "false", message = "Path is required" });
        }
    }
}
