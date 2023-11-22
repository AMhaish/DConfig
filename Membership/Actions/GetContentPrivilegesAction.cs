using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Membership.MembershipServices;
using Ninject;

namespace Membership.Actions
{
    public class GetContentPrivilegesAction : SABFramework.Core.SABAction
    {
        [Inject]
        private IContentPrivilegesAPI ContentPrivilegesAPI { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return Json(ContentPrivilegesAPI.GetPrivileges());
        }
    }
}
