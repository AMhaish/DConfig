using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public class UsersAPI : IUsersAPI
    {
        public virtual List<ApplicationUser> GetUsers()
        {
            MembershipDBContext context = new MembershipDBContext(true);
            var users = context.Users.ToList();
            return users;
        }

        public virtual List<ApplicationUser> GetCompanyUsers(int companyId)
        {
            MembershipDBContext context = new MembershipDBContext(true);
            var company = context.Companies.SingleOrDefault(m => m.Id == companyId);
            if (company != null)
            {
                return company.Users.ToList();
            }
            else
            {
                return null;
            }
        }

        public virtual ApplicationUser GetUser(string userName)
        {
            MembershipDBContext context = new MembershipDBContext(true);
            var user = context.Users.Where(m => m.UserName == userName).SingleOrDefault();
            return user;
        }

        public virtual ApplicationUser GetUserById(string userId)
        {
            MembershipDBContext context = new MembershipDBContext(true);
            var user = context.Users.Where(m => m.Id == userId).SingleOrDefault();
            return user;
        }

        public virtual List<IdentityUserRole> GetUserRoles(string userId)
        {
            MembershipDBContext context = new MembershipDBContext(true);
            var user = context.Users.Where(m => m.Id == userId).SingleOrDefault();
            return user.Roles.ToList();
        }

        public virtual List<ApplicationUser> GetUsersByName(string userName)
        {
            MembershipDBContext context = new MembershipDBContext(true);
            List<ApplicationUser> users;
            if (userName == "*")
            {
                users = context.Users.OrderBy(m => m.UserName).ToList();
            }
            else
            {
                users = context.Users.OrderBy(m => m.UserName).Where(m => m.UserName.Contains(userName)).ToList();
            }
            return users;
        }

    }
}
