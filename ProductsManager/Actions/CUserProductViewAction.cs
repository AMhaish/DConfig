using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using CompetitiveAnalysis.Models;
using CompetitiveAnalysis.ProductsManagerServices;
using DConfigOS_Core.Repositories.Utilities;
using System.IO;
using Ninject;

namespace CompetitiveAnalysis.Actions
{
    public class CUserProductViewAction : AppBaseAction
    {
        [Inject]
        public IUserProductViewsAPI userProductViewsAPI { get; set; }

        [Required]
        public int ProductId { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PostHandler(Controller controller)
        {
            if (controller.ModelState.IsValid && User!=null)
            {
                var result = userProductViewsAPI.RegisterView(User.Id,ProductId);
                switch (result)
                {
                    case ResultCodes.Succeed:
                        return Json(new { result = "true" });
                    default:
                        return Json(new { result = "false", message = "Product hasn't been found" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "Product Id not provided or the user isn't signed in" });
            }
        }

    }
}
