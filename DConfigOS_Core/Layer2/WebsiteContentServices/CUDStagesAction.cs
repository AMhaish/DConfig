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
    public class CUDStagesAction : UserActionsBase
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }        

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var stage = new Stage()
                {
                    Id = Id,
                    Name = Name                 
                };
                var result = stagesAPI.CreateStage(stage);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true", obj = stage });
                    case ResultCodes.ObjectAlreadyExists:
                        return Json(new { result = "false", message = "Stage with the same name is already exists" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to add a stage" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var stage = new Stage()
                {
                    Id = Id,
                    Name = Name
                };
                var result = stagesAPI.UpdateStage(stage);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        var obj = stagesAPI.GetStage(Id);
                        return Json(new { result = "true", obj = obj });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Stage hasn't been found to be updated" });          
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to update a stage" });
            }
            return null;
        }

        public override async Task<SABFramework.Core.SABActionResult> DeleteHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = stagesAPI.DeleteStage(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    case ResultCodes.ObjectHasntFound:
                        return Json(new { result = "false", message = "Stage hasn't been found to be deleted" });                    
                }
            }
            else
            {
                return Json(new { result = "false", message = "Id and Name are required to delete a stage" });
            }
            return null;
        }
    }
}
