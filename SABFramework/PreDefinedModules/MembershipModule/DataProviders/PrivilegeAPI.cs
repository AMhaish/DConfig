using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using SABFramework.PreDefinedModules.MembershipModule.Models;

namespace SABFramework.PreDefinedModules.MembershipModule.DataProviders
{
    public class PrivilegeAPI : IPrivilegeAPI
    {
        public virtual List<Privilege> GetPrivileges()
        {
            MembershipDBContext context = new MembershipDBContext();
            var prvs = context.Privileges.ToList();
            return prvs;
        }

        public virtual Privilege GetPrivilege(int id)
        {
            MembershipDBContext context = new MembershipDBContext();
            var p = context.Privileges.Where(m => m.Id == id).SingleOrDefault();
            return p;
        }

        
        public virtual bool CreatePrivilege(Privilege p)
        {
          
            MembershipDBContext context = new MembershipDBContext();
            var dbPrivilege = context.Privileges.Where(m => m.Controller == p.Controller && m.Action == p.Action && m.RequestType == p.RequestType).FirstOrDefault();
            if (dbPrivilege == null)
            {
                context.Privileges.Add(p);
                if (!String.IsNullOrEmpty(p.Action))
                {
                    var parents = context.Privileges.Where(m => m.Controller == p.Controller).ToList();
                    if (!String.IsNullOrEmpty(p.RequestType))
                    {
                        var parent = parents.Where(m => m.RequestType == null).First();
                        if (parent.Roles.Count > 0)
                        {
                            p.Roles = new List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>();
                            foreach (var r in parent.Roles)
                            {
                                p.Roles.Add(r);
                            }
                        }
                    }
                    else
                    {
                        var parent = parents.Where(m => m.Action == null).First();
                        if (parent.Roles.Count > 0)
                        {
                            p.Roles = new List<Microsoft.AspNet.Identity.EntityFramework.IdentityRole>();
                            foreach (var r in parent.Roles)
                            {
                                p.Roles.Add(r);
                            }
                        }
                    }
                }
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool UpdatePrivilege(Privilege p)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbPrivilege = context.Privileges.Where(m => m.Id == p.Id).FirstOrDefault();
            if (dbPrivilege != null)
            {
                dbPrivilege.NeedAuthentication = p.NeedAuthentication;
                dbPrivilege.NeedAuthorization = p.NeedAuthorization;
                dbPrivilege.RequireHttps = p.RequireHttps;
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual bool DeletePrivilege(int id)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbPrivilege = context.Privileges.Where(m => m.Id == id).FirstOrDefault();
            if (dbPrivilege != null)
            {
                context.Privileges.Remove(dbPrivilege);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool AddPrivilegeToRole(int PrivilegeId, string RoleName)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbPrivilege = context.Privileges.Where(m => m.Id == PrivilegeId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Name == RoleName).FirstOrDefault();
            if (dbPrivilege != null && dbRole != null)
            {
                dbPrivilege.Roles.Add(dbRole);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool RemovePrivilegeFromRole(int PrivilegeId, string RoleName)
        {
            MembershipDBContext context = new MembershipDBContext();
            var dbPrivilege = context.Privileges.Where(m => m.Id == PrivilegeId).FirstOrDefault();
            var dbRole = context.Roles.Where(m => m.Name == RoleName).FirstOrDefault();
            if (dbPrivilege != null && dbRole != null)
            {
                dbPrivilege.Roles.Remove(dbRole);
                context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
