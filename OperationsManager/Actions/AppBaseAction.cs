using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using OperationsManager.Models;
using OperationsManager.OperationsManagerServices;
using DConfigOS_Core.Repositories.Utilities;


namespace OperationsManager.Actions
{
    public class AppBaseAction : SABFramework.Core.SABAction
    {
        public static SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser User
        {
            get
            {
                if (HttpContext.Current.Session["User"] == null)
                {
                    var user = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.UsersAPI.GetUser(HttpContext.Current.User.Identity.Name);
                    HttpContext.Current.Session["User"] = user;
                }
                return (SABFramework.PreDefinedModules.MembershipModule.Models.ApplicationUser)HttpContext.Current.Session["User"];
            }
        }


        public override async Task<SABActionResult> GetHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }

        public override async Task<SABActionResult> PostHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }

        public override async Task<SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }

        public override async Task<SABActionResult> DeleteHandler(System.Web.Mvc.Controller controller)
        {
            return null;
        }
    }
}
