using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SABFramework.PreDefinedModules.MembershipModule.Models;
using Membership.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Repositories;

namespace Membership.MembershipServices
{
    public class UsersAPI : RepositoryBase<DConfigOS_Core_DBContext>, IUsersAPI
    {
        public virtual List<ApplicationUser> GetUsers()
        {
            Membership_DBContext context = new Membership_DBContext();
            var users = context.ExApplicationsUsers.Select(m => m.User).ToList();
            return users;
        }

        public virtual ApplicationUser GetUser(string userName)
        {
            Membership_DBContext context = new Membership_DBContext();
            var user = context.ExApplicationsUsers.Where(m => m.User.UserName == userName).Select(m => m.User).SingleOrDefault();
            return user;
        }

        public virtual ApplicationUser GetUserById(string userId)
        {
            Membership_DBContext context = new Membership_DBContext();
            var user = context.ExApplicationsUsers.Where(m => m.UserId==userId).Select(m => m.User).SingleOrDefault();
            return user;
        }

        public virtual List<IdentityUserRole> GetUserRoles(string userId)
        {
            Membership_DBContext context = new Membership_DBContext();
            var user = context.ExApplicationsUsers.Where(m => m.UserId == userId).Select(m => m.User).SingleOrDefault();
            return user.Roles.ToList();
        }

        public virtual List<ApplicationUser> GetUsersByName(string userName)
        {
            Membership_DBContext context = new Membership_DBContext();
            List<ApplicationUser> users;
            if (userName == "*")
            {
                users = context.ExApplicationsUsers.Select(m => m.User).ToList();
            }
            else {
                users = context.ExApplicationsUsers.Where(m => m.User.UserName.Contains(userName)).Select(m => m.User).ToList();
            }
            return users;
        }

        public virtual int CreateExUser(ExApplicationUser user)
        {
            if(!String.IsNullOrEmpty(user.UserId))
            {
                Membership_DBContext context = new Membership_DBContext();
                context.ExApplicationsUsers.Add(user);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.MissingInfo;
            }
        }

        public virtual int UpdateExUser(ExApplicationUser user)
        {
            if (!String.IsNullOrEmpty(user.UserId))
            {
                Membership_DBContext context = new Membership_DBContext();
                var dbuser = context.ExApplicationsUsers.SingleOrDefault(m => m.UserId == user.UserId);
                if(dbuser!=null)
                {
                    dbuser.ResourcesRootPath = user.ResourcesRootPath;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectHasntFound;
                }
            }
            else
            {
                return ResultCodes.MissingInfo;
            }
        }
    }
}
