using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using System.Security.Claims;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class AuthorizeAction : SABFramework.Core.SABAction
    {
        public async override Task<SABActionResult> GetHandler(Controller controller)
        {
            return HttpStatusCode(System.Net.HttpStatusCode.OK);

        }
    }
}
