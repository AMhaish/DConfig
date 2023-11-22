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
    public class UDCompanyUsers : SABFramework.Core.SABAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }

        public int Id { get; set; }
        public string UserId { get; set; }
        public List<string> UsersIds { get; set; }

        public override async Task<SABFramework.Core.SABActionResult> PutHandler(System.Web.Mvc.Controller controller)
        {
            if (UsersIds != null)
            {
                var result = companiesAPI.AddUsersToCompany(Id, UsersIds);
                if (result == true)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "Couldn't find the company to be updated, or couldn't find the users you want to add to the company" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No users Ids posted to be added" });
            }
        }

        public override async Task<SABActionResult> DeleteHandler(Controller controller)
        {
            if (!String.IsNullOrEmpty(UserId))
            {
                var result = companiesAPI.RemoveUsersFromCompany(Id, UserId);
                if (result == true)
                {
                    return Json(new { result = "true" });
                }
                else
                {
                    return Json(new { result = "false", message = "Couldn't find the company to be updated, or couldn't find the users you want to add to the company" });
                }
            }
            else
            {
                return Json(new { result = "false", message = "No users Ids posted to be deleted" });
            }
        }
    }
}
