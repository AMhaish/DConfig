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
using CompetitiveAnalysis.Models;
using DConfigOS_Core.Repositories.Utilities;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public class PropertiesEnumsAPI: IPropertiesEnumsAPI
    {

        public virtual List<PropertyEnum> GetPropertiesEnums()
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var enums = context.PropertyEnums.OrderBy(m => m.Name).ToList();
            return enums;
        }

        public virtual PropertyEnum GetPropertiesEnum(int id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var e = context.PropertyEnums.Where(m => m.Id == id).SingleOrDefault();
            return e;
        }

        public virtual int CreatePropertyEnum(PropertyEnum propertyEnum)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbEnum = context.PropertyEnums.Where(m => m.Name == propertyEnum.Name).FirstOrDefault();
            if (dbEnum == null)
            {
                context.PropertyEnums.Add(propertyEnum);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectNameAlreadyUsed;
            }
        }

        public virtual int UpdatePropertyEnum(PropertyEnum propertyEnum)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbEnum = context.PropertyEnums.Where(m => m.Id == propertyEnum.Id).FirstOrDefault();
            var dbSameEnum = context.PropertyEnums.Where(m => m.Name == propertyEnum.Name && m.Id != propertyEnum.Id).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbSameEnum == null)
                {
                    dbEnum.Name = propertyEnum.Name;
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

        public virtual int DeletePropertyEnum(int id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbEnum = context.PropertyEnums.Where(m => m.Id == id).FirstOrDefault();
            if (dbEnum != null)
            {
                if (dbEnum.Properties.Count <= 0)
                {
                    context.PropertyEnums.Remove(dbEnum);
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

        public virtual int UpdatePropertyEnumValues(int enumId, List<PropertyEnumValue> values)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbEnum = context.PropertyEnums.Where(m => m.Id == enumId).FirstOrDefault();
            if (dbEnum != null)
            {
                var oldValues = context.PropertyEnumValues.Where(w => w.EnumId == dbEnum.Id).ToList();
                foreach (var val in oldValues)
                {
                    context.PropertyEnumValues.Remove(val);
                }
                if (values != null)
                {
                    foreach (PropertyEnumValue value in values)
                    {
                        dbEnum.Values.Add(new PropertyEnumValue() { Value = value.Value , Weight=value.Weight, Priority=value.Priority});
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

        public virtual int DeletePropertyEnumValue(int id)
        {
            ProductsManager_DBContext context = new ProductsManager_DBContext();
            var dbValue = context.PropertyEnumValues.Where(m => m.Id == id).FirstOrDefault();
            if (dbValue != null)
            {
                context.PropertyEnumValues.Remove(dbValue);
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
