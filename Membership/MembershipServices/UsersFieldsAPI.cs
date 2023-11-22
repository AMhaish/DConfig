using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Membership.Models;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Models;

namespace Membership.MembershipServices
{
    public class UsersFieldsAPI : IUsersFieldsAPI
    {
        public virtual List<UserField> GetUsersFields()
        {
            Membership_DBContext context = new Membership_DBContext();
            var fields = context.UserFields.ToList();
            return fields;
        }

        public virtual List<FieldsType> GetUserFieldTypes()
        {
            Membership_DBContext context = new Membership_DBContext();
            var types = context.FieldsTypes.OrderBy(m => m.Name).ToList();
            return types;
        }

        public virtual int AddUserField(UserField field)
        {
            Membership_DBContext context = new Membership_DBContext();
            var sameField = context.UserFields.Where(m => m.Name == field.Name).FirstOrDefault();
            if (sameField == null)
            {
                //field.Enum = context.UserFieldEnums.SingleOrDefault(m => m.Id == field.EnumId);
                context.UserFields.Add(field);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int UpdateUserField(UserField field)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbField = context.UserFields.Where(m => m.Id == field.Id).FirstOrDefault();
            var sameField = context.UserFields.Where(m => m.Name == field.Name && m.Id != field.Id).FirstOrDefault();
            if (dbField != null)
            {
                if (sameField == null)
                {
                    dbField.Name = field.Name;
                    dbField.Type = field.Type;
                    dbField.EnumId = field.EnumId;
                    dbField.Priority = field.Priority;
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

        public virtual int DeleteUserField(int fieldId)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbField = context.UserFields.Where(m => m.Id == fieldId).FirstOrDefault();
            if (dbField != null)
            {
                context.UserFields.Remove(dbField);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int UpdateUserFieldValue(string userId, int fieldId, string value)
        {
            Membership_DBContext context = new Membership_DBContext();
            var dbUser = context.Users.Where(m => m.Id == userId).FirstOrDefault();
            var dbField = context.UserFields.Where(m => m.Id == fieldId).FirstOrDefault();
            var dbFieldValue = dbField.FieldValues.Where(m => m.UserId == userId).FirstOrDefault();
            if (dbUser != null && dbField != null)
            {
                if (dbFieldValue != null)
                {
                    dbFieldValue.Value = value;
                }
                else
                {
                    var newValue = new UserFieldsValue() { Value = value, UserId = userId };
                    dbField.FieldValues.Add(newValue);
                }
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual List<UserField> GetUserFieldsValues(string userId)
        {
            Membership_DBContext context = new Membership_DBContext();
            List<UserField> result = new List<UserField>();
            var records = from a in context.UserFields
                          from b in context.UserFieldsValues.Where(m => m.UserId==userId && a.Id==m.FieldId).DefaultIfEmpty()
                          select new { field = a, value = b.Value };
            foreach (var d in records.ToList())
            {
                result.Add(new UserField() { Id = d.field.Id, Name = d.field.Name, Type = d.field.Type, Value = d.value, EnumId=d.field.EnumId, Enum=d.field.Enum });
            }
            return result.ToList();
        }
    }
}
