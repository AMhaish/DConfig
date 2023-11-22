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
using Ninject;
using SABFramework.PreDefinedModules.MembershipModule.DataProviders;

namespace CompetitiveAnalysis.Actions
{
    public class AppBaseAction : SABFramework.Core.SABAction
    {
        [Inject]
        public ICompaniesAPI companiesAPI { get; set; }

        public IAdvancedPrivilegesAPI AdvancedPrivilegesAPI { get; set; }
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

        public virtual List<AdvancedPrivilege> UserAdvancedPrivileges
        {
            get
            {
                if (HttpContext.Current.Session["UserAdvancedPrivileges"] == null)
                {
                    var companies = companiesAPI.GetUserCompanies(User.Id);
                    var aps = AdvancedPrivilegesAPI.GetCompaniesPrivileges(companies.Select(m=>m.Id).ToList());
                    HttpContext.Current.Session["UserAdvancedPrivileges"] = aps;
                }
                return (List<AdvancedPrivilege>)HttpContext.Current.Session["UserAdvancedPrivileges"];
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
    }
}
