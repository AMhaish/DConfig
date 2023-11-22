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
using DConfigOS_Core.Repositories.IOServices;
using DConfigOS_Core.Repositories.Utilities;
using Ninject;

namespace DConfigOS_Core.Layer2.IOServices
{
    public class CUDResourcesFoldersAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IFoldersAPI foldersAPI { get; set; }

        [Required]
        public string Path { get; set; }
        public string NewPath { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = await foldersAPI.CreateFolder(Path);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Folder with the same name is already exists" });
                    default:
                        return Json(new { result = "false", message="Unknown error" });
                }
            }
            return Json(new { result = "false", message = "Path is required" });
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && !String.IsNullOrEmpty(NewPath))
            {
                var result = await foldersAPI.MoveFolder(Path,NewPath);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Folder hasn't been found to be updated" });
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
                var result = await foldersAPI.DeleteFolder(Path);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Folder hasn't been found to be deleted" });
                    default:
                        return Json(new { result = "false", message = "Unknown error" });
                }
            }
            return Json(new { result = "false", message = "Path is required" });
        }
    }
}
