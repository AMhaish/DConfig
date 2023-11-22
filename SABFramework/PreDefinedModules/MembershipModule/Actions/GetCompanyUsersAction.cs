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
using Microsoft.Owin.Security;
using System.Security.Claims;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class GetCompanyUsersAction : SABFramework.Core.SABAction
    {
        [Inject]
        public IUsersAPI usersAPI { get; set; }
        public int CompanyId { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            return Json(usersAPI.GetCompanyUsers(CompanyId));
        }
    }
}
