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
    public class UserFieldEnumsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IUserFieldsEnumsAPI
    {

        public virtual List<UserFieldEnum> GetUserFieldEnums()
        {
            Membership_DBContext context = new Membership_DBContext();
            var enums = context.UserFieldEnums.OrderBy(m => m.Name).ToList();
            return enums;
        }

        public virtual UserFieldEnum GetUserFieldEnum(int id)
        {
            Membership_DBContext context = new Membership_DBContext();
            var e = context.UserFieldEnums.Where(m => m.Id == id).SingleOrDefault();
            return e;
        }

        public virtual int CreateUserFieldEnum(UserFieldEnum userFieldEnum)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbEnum = context.UserFieldEnums.Where(m => m.Name == userFieldEnum.Name).FirstOrDefault();
            if (dbEnum == null)
            {
                context.UserFieldEnums.Add(userFieldEnum);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int UpdateUserFieldEnum(UserFieldEnum userFieldEnum)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbEnum = context.UserFieldEnums.Where(m => m.Id == userFieldEnum.Id).FirstOrDefault();
            var dbSameEnum = context.UserFieldEnums.Where(m => m.Name == userFieldEnum.Name && m.Id != userFieldEnum.Id).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbSameEnum == null)
                {
                    dbEnum.Name = userFieldEnum.Name;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteUserFieldEnum(int id)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbEnum = context.UserFieldEnums.Where(m => m.Id == id).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbEnum.UserFields.Count <= 0)
                {
                    context.UserFieldEnums.Remove(dbEnum);
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectLinkedToAnotherObject;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateUserFieldEnumValues(int enumId, List<string> values)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbEnum = context.UserFieldEnums.Where(m => m.Id == enumId).FirstOrDefault();
            if (dbEnum != null)
            {
                var oldValues = context.UserFieldEnumsValues.Where(w => w.EnumId == dbEnum.Id).ToList();
                foreach (var val in oldValues)
                {
                    context.UserFieldEnumsValues.Remove(val);
                }
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        dbEnum.Values.Add(new UserFieldEnumValue() { Value = value });
                    }
                }
                context.SaveChanges();
                return ResultCodes.Succeed;

            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteUserFieldEnumValue(int id)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbValue = context.UserFieldEnums.Where(m => m.Id == id).FirstOrDefault();
            if (dbValue != null)
            {
                context.UserFieldEnums.Remove(dbValue);
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
