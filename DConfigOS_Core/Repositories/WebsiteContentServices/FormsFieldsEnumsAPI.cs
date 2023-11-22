using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class FormsFieldsEnumsAPI : IFormsFieldsEnumsAPI
    {

        public virtual List<FormsFieldsEnum> GetFormsFieldsEnums(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var enums = context.FormsFieldsEnums.Where(m => creatorId == null || m.CreatorId == creatorId).OrderBy(m => m.Name).ToList();
            return enums;
        }

        public virtual FormsFieldsEnum GetFormsFieldsEnum(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var e = context.FormsFieldsEnums.Where(m => m.Id==id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
            return e;
        }

        public virtual int CreateFormsFieldsEnum(FormsFieldsEnum fieldsEnum, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.FormsFieldsEnums.Where(m => m.Name == fieldsEnum.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum == null)
            {
                context.FormsFieldsEnums.Add(fieldsEnum);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateFormsFieldsEnum(FormsFieldsEnum fieldsEnum, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.FormsFieldsEnums.Where(m => m.Id == fieldsEnum.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var dbSameEnum = context.FormsFieldsEnums.Where(m => m.Name == fieldsEnum.Name && m.Id != fieldsEnum.Id).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbSameEnum == null)
                {
                    dbEnum.Name = fieldsEnum.Name;
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

        public virtual int DeleteFormsFieldsEnum(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.FormsFieldsEnums.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbEnum.FormFields.Count <= 0)
                {
                    context.FormsFieldsEnums.Remove(dbEnum);
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
        public virtual int UpdateFormsFieldsEnumValues(int enumId, List<string> values, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbEnum = context.FormsFieldsEnums.Where(m => m.Id == enumId && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbEnum != null)
            {
                var oldValues = context.FormsFieldsEnumsValues.Where(w => w.EnumId == dbEnum.Id).ToList();
                foreach (var val in oldValues)
                {
                    context.FormsFieldsEnumsValues.Remove(val);
                }
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        dbEnum.Values.Add(new FormsFieldsEnumValue() { Value = value });
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

        public virtual int DeleteFormsFieldsEnumValue(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbValue = context.FormsFieldsEnumsValues.Where(m => m.Id == id && (creatorId == null || m.Enum.CreatorId == creatorId)).FirstOrDefault();
            if (dbValue != null)
            {
                context.FormsFieldsEnumsValues.Remove(dbValue);
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
