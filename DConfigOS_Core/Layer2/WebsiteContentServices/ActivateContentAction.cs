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
    public class ActivateContentAction : UserActionsBase
    {
        [Required]
        public int Id { get; set; }

        [Inject]
        public IContentsAPI contentsAPI { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> GetHandler(Controller controller)
        {
            if (controller.ModelState.IsValid)
            {
                var result = contentsAPI.ActivateContent(Id);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return View("Content have been activated successfully" );
                    case ResultCodes.ObjectAlreadyUpdated:
                        return View("Content have been already activated");
                    case ResultCodes.ObjectHasntFound:
                    default:
                        return View("Content hasn't been found to be activated" );
                }
            }
            else
            {
                return View("Id is required for activating");
            }
        }

    }
}
