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
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;

namespace SABFramework.PreDefinedModules.MembershipModule.Actions
{
    public class GetRolesAction : SABFramework.Core.SABAction
    {
        public string RoleName { get; set; }
        public override async Task<SABActionResult> GetHandler(Controller controller)
        {
            MembershipDBContext dbContext = new MembershipDBContext();
            if (String.IsNullOrEmpty(RoleName))
            {
                var list = new List<IdentityRole>();
                var returnedList = new List<IdentityRole>();
                list = dbContext.Roles.ToList();
                foreach (IdentityRole ir in list)
                {
                    if (ir.Name != "Administrators")
                    {
                        returnedList.Add(new IdentityRole() { Name = ir.Name, Id = ir.Id });
                    }
                    else
                    {
                        if (MembershipProvider.Instance.CurrentUserIsAdministrator)
                        {
                            returnedList.Add(new IdentityRole() { Name = ir.Name, Id = ir.Id });
                        }
                    }
                }
                return Json(returnedList);
            }
            else
            {
                var role = dbContext.Users.Where(m => m.UserName == RoleName).FirstOrDefault();
                return Json(role);
            }
        }
    }
}
