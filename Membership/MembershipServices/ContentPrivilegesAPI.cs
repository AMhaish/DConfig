using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Membership.Models;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Repositories;
using DConfigOS_Core.Models;

namespace Membership.MembershipServices
{
    public class ContentPrivilegesAPI : RepositoryBase<DConfigOS_Core_DBContext>, IContentPrivilegesAPI
    {
        public virtual List<ContentPrivilege> GetPrivileges()
        {
            Membership_DBContext context = new Membership_DBContext();
            var prvs = context.ContentPrivileges.ToList();
            return prvs;
        }

        public virtual ContentPrivilege GetPrivilege(int contentId)
        {
            Membership_DBContext context = new Membership_DBContext();
            var p = context.ContentPrivileges.Where(m => m.ContentId==contentId).SingleOrDefault();
            return p;
        }


        public virtual int CreatePrivilege(ContentPrivilege p)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbPrivilege = context.ContentPrivileges.Where(m => m.ContentId == p.ContentId).FirstOrDefault();
            if (dbPrivilege == null)
            {
                context.ContentPrivileges.Add(p);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdatePrivilege(ContentPrivilege p)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbPrivilege = context.ContentPrivileges.Where(m => m.ContentId == p.ContentId).FirstOrDefault();
            if (dbPrivilege != null)
            {
                dbPrivilege.NeedAuthentication = p.NeedAuthentication;
                dbPrivilege.NeedAuthorization = p.NeedAuthorization;
                dbPrivilege.RequireHttps = p.RequireHttps;
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
        public virtual int DeletePrivilege(int contentId)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbPrivilege = context.ContentPrivileges.Where(m => m.ContentId == contentId).FirstOrDefault();
            if (dbPrivilege != null)
            {
                context.ContentPrivileges.Remove(dbPrivilege);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int AddPrivilegeToRole(int contentId,string RoleName)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbPrivilege = context.ContentPrivileges.Where(m => m.ContentId == contentId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Name == RoleName).FirstOrDefault();
            if (dbPrivilege != null && dbRole!=null)
            {
                dbPrivilege.Roles.Add(dbRole);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int RemovePrivilegeFromRole(int contentId, string RoleName)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbPrivilege = context.ContentPrivileges.Where(m => m.ContentId == contentId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Name == RoleName).FirstOrDefault();
            if (dbPrivilege != null && dbRole != null)
            {
                dbPrivilege.Roles.Remove(dbRole);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }
    }
}
