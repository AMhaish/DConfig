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
    public class GetUserCompaniesAction : SABFramework.Core.SABAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }

        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            var coms= companiesAPI.GetUserCompanies(SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserId);
            foreach(var c in coms)
            {
                if (MembershipProvider.Instance.ContextCompanyId==c.Id)
                {
                    c.Current = true;
                    break;
                }
            }
            return Json(coms);
        }
    }
}
