using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SABFramework.Core;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Ninject;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class SetCurrentContextAction : SABFramework.Core.SABAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }

        public int Id { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            var list = companiesAPI.GetUserCompanies(MembershipProvider.Instance.CurrentUserId);
            if(list.Select(m => m.Id).Contains(Id))
            {
                MembershipProvider.Instance.ContextCompanyId = Id;
                return Json(new { result = "true" });
            }
            else
            {
                return Json(new { result = "false", message = "You don't have access to that instance" });
            }
        }
    }
}
